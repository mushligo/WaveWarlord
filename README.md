# WaveWarlord — 从零重建

当前阶段：R0工程准备与R1船长移动训练。游戏设计延续V2世界系统和V3海战规则；双人各控一船，世界进度保存于房主。

## 打开与运行

- 编辑器：**Unity 2022.3.53f1c1**。
- 场景：`Assets/_Game/Scenes/HarborLab.unity`。
- 在Unity Hub打开本工程，等待导入与编译，打开HarborLab后点击Play。
- 当前胶囊Captain保持静止，移动首版是学习任务。仓库、告示板和码头是灰盒占位，没有交互逻辑。
- Inspector中的CharacterController承担后续移动碰撞；固定镜头用于第一周观察。

## 构建与验证

菜单 `WaveWarlord > R0 > Validate Harbor Lab` 检查场景引用、CharacterController、URP及构建场景。

菜单 `WaveWarlord > R0 > Build Windows` 构建到 `Builds/Windows/R0/WaveWarlord.exe`。普通启动显示灰盒；关闭窗口退出。

使用验证或构建菜单前，先保存正在编辑的场景；工具会打开HarborLab。

开发构建可附加 `--r0-smoke-test`，执行启动检查、保存主相机渲染图与结果到构建旁的SmokeEvidence后退出。检查使用URP渲染纹理，便于在隐藏窗口中验证场景画面；交互操作仍需在编辑器或普通程序窗口中检查。该开关不代表移动、交互或网络已经完成。

## 文件用途

| 位置 | 用途 |
|---|---|
| Assets/_Game/Scenes | 港口灰盒 |
| Assets/_Game/Scripts | 学习者编写玩法代码的起点 |
| Assets/_Game/Editor/HarborSetup.cs | AI编写的建场景、验证与构建工具 |
| Assets/_Game/Diagnostics/R0SmokeCheck.cs | AI编写的开发构建启动检查，普通运行不启用 |
| Assets/_Game/Settings、Art/Materials | 本工程新建的URP配置与灰盒材质 |
| Documentation/第一周任务卡.md | 当前任务与验收记录 |

`HarborSetup.CreateHarborLab`遇到已有场景会停止，避免覆盖学习者修改。日常只使用验证和构建入口。

## 设计与旧实现

- 当前计划：`D:\GameDesign\document\plans\WaveWarlord_从零重建与独立开发训练方案.md`
- 实际进度：`D:\GameDesign\document\plans\WaveWarlord_执行资料\00_当前进度.md`
- 旧工程归档：`D:\Develop\UnityProject\_Archives\WaveWarlord_20260928_201206\Project`
- 备份校验：归档上级的`verification.json`、`git-status.txt`、`git-head.txt`。

新工程使用独立Git历史。旧项目的修改与历史保存在归档，不混入新工程基线。提交Assets及meta、Packages清单及锁文件、ProjectSettings和文档；忽略Library、Temp、Logs、UserSettings和Builds。

场景与工具由AI完成，不作为学习者已掌握的证明。核心首版由学习者尝试，随后审查、排错和独立修改。
