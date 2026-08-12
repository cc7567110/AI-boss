# AI Boss V0.1

一个帮助自己提升专注力和执行力的 Windows 学习助手。

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

## 隐私与联网

AI Boss V0.1 不使用账号、云端、数据库、OpenAI API 或网络服务。程序运行时不会联网，学习数据只保存在你的电脑上。

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
│  ├─ Forms/                  # 任务和工作轮次的弹窗
│  ├─ Models/                 # 任务、日志、规则等数据结构
│  └─ Services/               # 本地 JSON 保存逻辑
├─ NuGet.Config               # .NET 构建时的官方包源配置
├─ README.md                  # 项目说明
└─ .gitignore                 # Git 忽略规则
```

## 开发技术

- C# / .NET 10
- Windows Forms（Windows 原生桌面界面）
- JSON 本地文件存储
- 不使用第三方程序库

## GitHub 发布说明

`.gitignore` 已排除编译输出、开发缓存、本机数据、备份文件和可能的密钥文件。将项目上传到 GitHub 前，建议再次确认 `git status` 的文件列表中没有个人学习记录或导出的 JSON 备份。

本项目目前未附带开源许可证；在公开发布前，请根据你希望他人如何使用和修改代码，选择并添加合适的许可证。
