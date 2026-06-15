# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

DeskPilot 是一个轻量级 Windows 桌面整理工具，基于 .NET 8 + WPF + MVVM 架构开发。

核心功能：
- 自动整理 Windows 桌面文件（规则整理 + AI 自然语言整理）
- 飞书待办和日程显示
- 桌面悬浮组件 + 系统托盘 + 独立主窗口三种形态

## 开发环境要求

- .NET 8 SDK
- Windows 10/11
- Visual Studio 2022 或 VS Code + C# Dev Kit

## 项目结构

```
DeskPilot/
  src/
    DeskPilot.App/          # WPF 主应用（Views, ViewModels, App.xaml）
    DeskPilot.Core/         # 核心业务逻辑（文件扫描、整理引擎、AI服务接口）
    DeskPilot.Infrastructure/  # 基础设施（LarkCliService, 设置存储, Toast通知）
    DeskPilot.Tests/        # 单元测试（xUnit）
  docs/
    interaction/            # 交互设计稿
    visual/                 # 视觉设计稿
    architecture/            # 架构文档
```

## 常用命令

```bash
# 还原依赖
dotnet restore

# 构建（Debug）
dotnet build

# 构建（Release）
dotnet build -c Release

# 运行测试
dotnet test

# 运行单个测试
dotnet test --filter "FullyQualifiedName~TestClassName.MethodName"

# 启动应用
dotnet run --project src/DeskPilot.App
```

## 架构要点

### MVVM 架构
- ViewModels 放在 `DeskPilot.App/ViewModels/`
- Views 放在 `DeskPilot.App/Views/`
- 使用 CommunityToolkit.Mvvm 简化 MVVM 实现
- ViewModels 不引用 Views，通过数据绑定交互

### 关键服务（接口 + 实现）
- `IFileOrganizerService` / `FileOrganizerService` - 文件扫描与整理
- `IOrganizePlanService` - 整理计划生成（规则 + AI）
- `IAiPlanningService` - AI 整理能力
- `ILarkCliService` - lark-cli 封装（子进程调用）
- `ISettingsService` - 用户设置存储
- `IThemeService` - 主题管理
- `IAutoStartService` - 开机启动管理

### Lark CLI 封装
通过子进程调用 `lark-cli`，统一由 `LarkCliService` 封装：
- JSON 输出优先解析
- 捕获 stdout/stderr/exit code
- 设置超时，支持取消
- 不在日志中暴露 token/secret

### 文件整理安全机制
1. 先扫描 → 生成计划 → 预览 → 用户确认 → 执行
2. 不删除文件，不覆盖文件
3. 快捷方式/隐藏文件/系统文件默认跳过
4. 同名文件自动重命名 `文件名 (1).ext`
5. 支持撤销上一次整理

### AI 集成
- 仅发送文件元信息（名称、扩展名、大小、时间）
- 不发送文件正文、绝对路径中的用户名
- 支持 OpenAI-Compatible API
- API Key 本地安全存储（DPAPI）
- 网络失败时降级为规则整理

### 飞书数据刷新
- 程序启动立即刷新一次
- 待办/日程每 10 分钟刷新
- 用户可手动刷新
- 刷新失败保留上一次成功数据

## 主题支持

必须支持浅色/深色/跟随系统三种模式，使用 Windows 11 Fluent Design 风格，避免复杂动效和强运营感。

## TDD 要求

必须先写测试再写实现，核心测试覆盖：
- 文件扫描与分类
- 快捷方式/隐藏文件跳过
- 同名文件重命名
- AI Prompt 生成与 JSON 解析
- lark-cli 输出解析
- 设置保存与读取
