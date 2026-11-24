# 京杭筑迹

![《京杭筑迹》游戏封面](Assets/%E5%9B%BE%E7%89%87%E7%B4%A0%E6%9D%90/%E5%B0%81%E9%9D%A2.jpg)

《京杭筑迹》是一款以京杭大运河和中国传统营造技艺为主题的 **Unity 2D 剧情解谜游戏**。从北京一路南下到杭州，沿途参与斗拱组装、建筑修缮、园林探索和榫卯练习，最后动手搭一座西湖廊桥。

[五关路线](#五关路线) · [启动游戏](#启动游戏) · [工程结构](#工程结构)

## 五关路线

| 关卡 | 玩法 | 场景入口 |
| --- | --- | --- |
| 北京 | 攀爬、拼图和建筑知识答题，最后组装斗拱。 | [进入北京](Assets/Level1/Scenes/1_Beijing.unity) |
| 台儿庄 | 完成修缮任务，通过对话选择推进剧情。 | [台儿庄开场](Assets/Level2/Scenes/2_1_Taierzhuang.unity) |
| 扬州 | 用营造工具检查和修复台基，完成剧情问答。 | [扬州开场](Assets/Level3_YangzhouPavilion/Scenes/Level3_Intro.unity) |
| 苏州 | 探索荷池、山楼、长廊和雨亭，收集线索、完成拼图与搭建。 | [苏州开场](Assets/Level4_Suzhou/Scenes/Level4_Intro.unity) |
| 杭州 | 练习透榫、半榫、燕尾榫和馒头榫，再分阶段搭建西湖廊桥。 | [杭州开场](Assets/Level5_Hangzhou/Scenes/Level5_Scene1_Opening.unity) |

## 启动游戏

1. 克隆仓库，或在 GitHub 选择 **Code → Download ZIP**，下载后解压。
2. 在 Unity Hub 选择 **打开**，添加包含 `Assets`、`Packages` 和 `ProjectSettings` 的仓库根目录。
3. 使用 [工程指定的 Unity 2022.3.62f3c1](ProjectSettings/ProjectVersion.txt) 打开，等待资源和包导入。
4. 在 Project 面板打开 [`Assets/00_Cover.unity`](Assets/00_Cover.unity)，点击编辑器顶部的 **Play**，从封面进入游戏。

游戏内账号和关卡进度通过 Unity `PlayerPrefs` 保存在本机。这个版本的扬州问答使用预设反馈和固定档位奖励，体验剧情无需配置第三方 AI 密钥。

## 工程结构

| 位置 | 内容 |
| --- | --- |
| [`Assets/`](Assets) | 游戏场景、C# 脚本和美术、音频等资源。五关位于 `Level1`、`Level2`、`Level3_YangzhouPavilion`、`Level4_Suzhou`、`Level5_Hangzhou`。 |
| [`Packages/manifest.json`](Packages/manifest.json) | Unity 项目依赖。 |
| [`ProjectSettings/EditorBuildSettings.asset`](ProjectSettings/EditorBuildSettings.asset) | 构建场景列表，包含封面、地图、各关和结局。 |
