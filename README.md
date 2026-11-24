# 京杭筑迹

一个以京杭大运河沿线建筑为主题的 Unity 2D 游戏工程。五关依次为北京、台儿庄、扬州、苏州、杭州。

## 打开工程

用 Unity Hub 添加仓库根目录，并使用工程指定的 Unity **2022.3.62f3c1**。首次打开时等待 Unity 恢复项目依赖并导入资源，然后打开 `Assets/00_Cover.unity` 并进入 Play Mode。构建场景已配置在 `ProjectSettings/EditorBuildSettings.asset`。

## 离线功能

仓库已移除小墨联网助手、第三方 AI 凭据和调用，以及原发行平台的加密配置。扬州剧情保留问答与推进流程，使用本地预设反馈和固定档位奖励。游戏的本地账号和进度由 Unity PlayerPrefs 在运行时生成。
