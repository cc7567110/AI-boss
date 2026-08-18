# AI Boss V0.2.0

AI Boss 是一个离线运行的 Windows 学习执行助手，通过任务管理、专注计时、执行记录和复盘机制帮助用户建立学习执行系统。

它诞生于一个简单的问题：
“如果我总是无法按照计划行动，能不能创造一个系统来监督自己？”

AI Boss 不追求复杂的功能，而是帮助用户把模糊目标拆解成具体行动，并记录真实执行过程。

## 功能

- 管理一个当前任务：任务名称、目标和状态。
- 选择 30、40 或 45 分钟工作轮次。
- 开始、暂停、继续和结束计时。
- 结束轮次时记录计划产出、实际产出、开始时间、结束时间和实际时长。
- 使用 IDEA BOX 快速记下与当前任务无关的想法，不会影响正在进行的计时。
- 查看历次工作轮次的学习日志。
- 查看、编辑和启用/停用固定 Boss 规则。
- 将所有数据自动保存在本机 JSON 文件中，并支持手动导出完整 JSON 备份。

## IDEA BOX

说明：
- 用于记录学习过程中产生的临时想法。
- 避免打断当前学习任务。

V0.2 新增：
- 长文本查看优化
- 编辑功能
- 删除功能

## 每日学习日志

说明：
- 记录每次学习轮次的执行结果。

V0.2 新增：
- 长文本显示优化
- 完整内容查看
- 编辑功能
- 删除功能

## 使用说明

Windows 用户可以在 Releases 页面下载最新版本压缩包：

```text
AI-Boss-v0.2.0-Windows-x64.zip
```

解压后进入 `AI-Boss-v0.2.0` 文件夹，运行：

```text
AIBoss.exe
```

发布为自包含版本，使用者不需要另行安装 .NET Runtime。

## Changelog

### V0.2.0
- 优化 IDEA BOX 使用体验
- 优化每日学习日志体验
- 增加 IDEA BOX 编辑功能
- 增加 IDEA BOX 删除功能
- 增加学习日志编辑功能
- 增加学习日志删除功能
- 优化长文本内容查看

### V0.1.0
- 初始版本发布

## 数据与隐私

AI Boss V0.2.0 本地运行，不需要账号，不依赖云端，不使用 OpenAI API。程序运行时不会联网，用户数据只保存在本机。

默认数据位置：

```text
%LOCALAPPDATA%\AIBoss\ai-boss-data.json
```

也可以通过软件右上角的“打开数据文件夹”查看它。

## 运行环境

- Windows 10 或 Windows 11（64 位）
- 开发和编译：.NET 10 SDK
- 运行开发版：.NET 10 Desktop Runtime

发布为自包含 EXE 后，使用者不需要另行安装 .NET Runtime。

## 本地运行

在项目根目录打开 PowerShell，执行：

```powershell
dotnet run --project .\AIBoss\AIBoss.csproj
```

## 生成可发布的单文件 EXE

在项目根目录执行：

```powershell
dotnet publish .\AIBoss\AIBoss.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

完成后，可双击运行的文件位于：

```text
AIBoss\bin\Release\net10.0-windows\win-x64\publish\AIBoss.exe
```

这个 EXE 会包含 Windows 运行所需的 .NET 组件，因此文件较大是正常现象。发布到 GitHub 时，建议将它作为 GitHub Release（发行版）的附件，而不是提交到源码仓库。

## 项目结构

```text
AI-boss/
├─ AIBoss.sln                 # Visual Studio 解决方案入口
├─ AIBoss/
│  ├─ AIBoss.csproj           # .NET 项目和版本配置
│  ├─ Program.cs              # 程序启动入口
│  ├─ MainForm.cs             # 主窗口
│  ├─ Forms/                  # 任务、工作轮次、想法详情和学习日志详情的弹窗
│  ├─ Models/                 # 任务、日志、想法、规则等数据结构
│  └─ Services/               # 本地 JSON 保存逻辑
├─ NuGet.Config               # .NET 构建时的官方包源配置
├─ README.md                  # 项目说明
└─ .gitignore                 # Git 忽略规则
```

## 开发技术

- C# / .NET 10
- Windows Forms（Windows 原生桌面界面）
- System.Text.Json（本地 JSON 文件存储）
- 不使用第三方程序库

## 未来计划

未来可能增加：
- 更智能的学习反馈
- 学习数据分析
- 学习趋势统计
- AI 辅助监督能力

## GitHub 发布说明

`.gitignore` 已排除编译输出、开发缓存、本机数据、备份文件和可能的密钥文件。将项目上传到 GitHub 前，建议再次确认 `git status` 的文件列表中没有个人学习记录或导出的 JSON 备份。

本项目目前未附带开源许可证；在公开发布前，请根据你希望他人如何使用和修改代码，选择并添加合适的许可证。
