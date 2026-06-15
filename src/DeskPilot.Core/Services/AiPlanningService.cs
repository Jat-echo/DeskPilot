using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;

namespace DeskPilot.Core.Services;

public class AiPlanningService : IAiPlanningService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string GeneratePrompt(List<DesktopFile> files, string userRequest)
    {
        var fileList = string.Join("\n", files.Select(f =>
            $"  - {f.Name} | {f.Extension} | {FormatFileSize(f.Size)} | 创建于 {f.CreatedTime:yyyy-MM-dd} | 修改于 {f.ModifiedTime:yyyy-MM-dd}"));

        return $@"你是一个Windows桌面文件整理助手。用户希望你按照以下需求整理桌面文件：

用户需求：{userRequest}

桌面文件列表：
{fileList}

请生成一个JSON格式的整理计划，包含以下字段：
- foldersToCreate: 需要创建的文件夹列表
- moves: 文件移动计划，每个包含source(原文件名)、target(目标路径)、reason(原因)
- skipped: 跳过的文件，每个包含file(文件名)、reason(原因)
- warnings: 警告信息列表

重要规则：
1. 不要移动快捷方式(.lnk文件)
2. 不要移动隐藏文件
3. 不要移动系统文件
4. 同一目标文件夹下已有同名文件时，自动在文件名后加(1)、(2)等序号
5. 只返回JSON，不要包含其他文字

输出格式示例：
{{
  ""foldersToCreate"": [""项目文件夹""],
  ""moves"": [
    {{""source"": ""doc.pdf"", ""target"": ""项目文件夹/doc.pdf"", ""reason"": ""与项目相关""}}
  ],
  ""skipped"": [
    {{""file"": ""Chrome.lnk"", ""reason"": ""快捷方式默认不处理""}}
  ],
  ""warnings"": [""本次不会删除任何文件""]
}}";
    }

    public AiPlanningResult ParseResponse(string jsonResponse)
    {
        try
        {
            // Try to extract JSON from response (in case model returns extra text)
            var startIndex = jsonResponse.IndexOf('{');
            var endIndex = jsonResponse.LastIndexOf('}');
            if (startIndex >= 0 && endIndex > startIndex)
            {
                jsonResponse = jsonResponse.Substring(startIndex, endIndex - startIndex + 1);
            }

            var plan = JsonSerializer.Deserialize<OrganizePlan>(jsonResponse, JsonOptions);
            return new AiPlanningResult { Success = true, Plan = plan };
        }
        catch (JsonException ex)
        {
            return new AiPlanningResult { Success = false, ErrorMessage = $"JSON解析失败: {ex.Message}" };
        }
    }

    public async Task<AiPlanningResult> SendRequestAsync(List<DesktopFile> files, string userRequest, AiProviderSettings settings)
    {
        if (!settings.Enabled || string.IsNullOrEmpty(settings.ApiKey))
        {
            return new AiPlanningResult { Success = false, ErrorMessage = "AI 未配置或未启用" };
        }

        var prompt = GeneratePrompt(files, userRequest);

        var requestBody = new
        {
            model = settings.ModelName,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = settings.Temperature,
            max_tokens = settings.MaxTokens
        };

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds) };
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");

        var content = new StringContent(JsonSerializer.Serialize(requestBody, JsonOptions), Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync($"{settings.BaseUrl.TrimEnd('/')}/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new AiPlanningResult { Success = false, ErrorMessage = $"API请求失败: {response.StatusCode}" };
            }

            using var doc = JsonDocument.Parse(responseBody);
            var message = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return ParseResponse(message ?? "");
        }
        catch (TaskCanceledException)
        {
            return new AiPlanningResult { Success = false, ErrorMessage = "请求超时" };
        }
        catch (HttpRequestException ex)
        {
            return new AiPlanningResult { Success = false, ErrorMessage = $"网络错误: {ex.Message}" };
        }
        catch (Exception ex)
        {
            return new AiPlanningResult { Success = false, ErrorMessage = $"未知错误: {ex.Message}" };
        }
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
