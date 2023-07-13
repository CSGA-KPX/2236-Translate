# AD2236机翻

* 这是机翻！尽管会人工校对文本，但只保证“能看”级别的质量。可以提交issue修正。
* 目前项目只有一个人，按目前进度大概两三个月。
* 游戏资源写死了日文和中文。考虑到基本都学过英语，后续补丁会直接替换英语文本。（不涉及内嵌的文字）

# 进度
- [x] 测试工具环境
- [x] 谷歌机翻（英日）
- [x] GPT3.5机翻（日）
- [x] 词汇表
- [ ] 人工检查
    * 文本检查 @ ```2236_09.book|Assets/2236/2236_09.xlsx:PHASE9|22```
    * 游戏内检查 @ ```2236_06.book|Assets/2236/2236_06.xlsx:PHASE6|23```
- [ ] 完成 `2236utils lint` 检查
- [ ] 发布alpha补丁
- [ ] 解决Issues内问题
- [ ] 发布beta补丁

# 大致流程
1. 下载安装UABEA( https://github.com/nesrak1/UABEA ) 。只能使用UABEA，UABE导出的格式非常难解析，而AssetStudio无法编辑资源。
2. 以JSON格式从下列文件中导出脚本到 `uabea-dump/` ，分别在一下目录中。但实际测试中游戏使用的貌似是后者的文本。
    * ```2236 A.D. -Universal Edition-_Data\sharedassets0.assets\book*.json```
    * ```2236 A.D. -Universal Edition-_Data\StreamingAssets\2236\Windows\2236.scenarios.asset\book*.json```
3. 使用 `2236utils dump` 生成翻译项目文件。
4. 开始机翻
    * 谷歌翻译：
        1. 安装API环境（ https://github.com/nidhaloff/deep-translator-api ）。
        2. 使用 `RunAPI.cmd` 启动API环境。
        3. 使用 `2236utils googletransen` 或者 `2236utils googletransja`开始翻译。
        4. 谷歌翻译基本没有流量控制。
    * GPT翻译：
        1. 下载配置GalTrans环境（ https://github.com/XD2333/GalTransl ）。
        2. 使用 `2236utils GalTransDump` 生成GalTrans用的输入文件。
        3. 在GalTrans内进行翻译。
        4. 使用 `2236utils GalTransMerge` 将GalTrans生成的翻译结果汇入项目文件。
5. 人工编辑项目文件校对并填写Final字段。
6. 使用 `2236utils merge` 将翻译文本汇入UABEA导出的JSON文件。
7. 使用UABEA替换对应资源文件。
8. 删除资源缓存 `%USERPROFILE%\AppData\LocalLow\Unity\Chloro_2236 A_D_ -Universal Edition-` 。如果不删游戏不会加载更改内容。
9. 启动游戏测试。

备注：建议替换游戏字体为思源黑体，游戏内为小冢黑体对中文支持不佳。相关字体文件位于 `sharedassets0.assets` 中。

## 大致文本量

| 章节名   | 起始行 | 文本量 |
| -------- | ------ | ------ |
| Start    | 3      | 36     |
| PROLOGUE | 39     | 3663   |
| PHASE 1  | 3702   | 8397   |
| PHASE 2  | 12099  | 6057   |
| PHASE 3  | 18156  | 5859   |
| PHASE 4  | 24015  | 11133  |
| PHASE 5  | 35148  | 7947   |
| PHASE 6  | 43095  | 13014  |
| PHASE 7  | 56109  | 9459   |
| PHASE 8  | 65568  | 10665  |
| PHASE 9  | 76233  | 6831   |
| PHASE10  | 83064  | 6480   |
| PHASE11  | 89544  | 7245   |
| PHASE12  | 96789  | 6237   |
| PHASE13  | 103026 | 5571   |
| PHASE14  | 108597 | 10719  |
| PHASE15  | 119316 | 3942   |
| PHASE16  | 123258 | 5022   |
| PHASE16Y | 128280 | 3123   |
| PHASE17  | 131403 | 8217   |
| PHASE17Y | 139620 | 5373   |
| PHASE18  | 144993 | 28935  |
| PHASE18H | 173928 | 7047   |
| PHASE18Y | 180975 | 1899   |
| PHASE19  | 182874 | 9512   |


# 致谢

GPT翻译方案来自 https://github.com/XD2333/GalTransl

谷歌翻译工具来自 https://github.com/nidhaloff/deep-translator-api