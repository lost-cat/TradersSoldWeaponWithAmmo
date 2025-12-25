# Traders Sold Weapon With Ammo

为装有 Combat Extended 的玩家改进商人/据点货表：当商人出售 CE 武器时，同时在货表中提供对应弹药，避免“有枪无弹”的尴尬。

— Works with Combat Extended: when a trader sells a CE weapon, matching ammo stacks are added to their stock.

## 功能特性
- 自动检测商人/据点在生成货物时的 CE 武器，并补充一到数堆兼容弹药。
- 弹药数量遵循原版/CE 的堆叠生成规则（基于 `StockGeneratorUtility`），力求与原生体验一致。
- 以 Postfix 方式补丁，尽量减少对其他模组的冲突风险。

## 依赖与版本
- 必需：Combat Extended（CE）。
- RimWorld：建议 1.6（兼容性取决于 CE 版本）。
- 运行库：.NET Framework 4.8（构建 TargetFramework：`net480`）。

## 安装与加载顺序
1. 将本模组文件夹放入 RimWorld/Mods 目录。
2. 在模组加载顺序中，确保本模组位于 Combat Extended 之后。
3. 进入游戏后新开的商人/据点货物将生效（已生成的在下一次刷新后生效）。

## 与其他模组的兼容性
- 采用 Harmony Postfix 钩子补充货物，理论上与大多数交易相关模组兼容。
- 若其他模组也对同一生成流程进行“移除/替换”操作，可能影响最终货表结果；调整加载顺序可缓解。

## 实现说明（开发者）
- 关键补丁：`StockGenerator.GenerateThings(Faction)` Postfix，见源码 [Source/TraderStockPatches.cs](Source/TraderStockPatches.cs)。
- 主要流程：
	- 筛选生成结果中的 CE 武器（`CompAmmoUser`/CE 工具检测）。
	- 为每种武器挑选可用的弹药定义，生成合适的堆叠数量并加入结果集合。
- Harmony 初始化：见 [Source/Main.cs](Source/Main.cs)。
- 可选强依赖：在 `.vscode/mod.csproj` 中已加入对 CE 的条件引用，支持 Workshop 目录自动发现与手动覆盖。



## 常见问题
- 看不到弹药？请确认 CE 已启用，且本模组加载在 CE 之后；另外仅对新生成/刷新后的货表生效。
- 和其他交易修改模组冲突？尝试调整加载顺序，或联系作者提供冲突信息（日志、模组列表）。

## 致谢
- Combat Extended 团队与贡献者。
- Harmony by Andreas Pardeike。

## English Summary
- Adds matching ammo to trader/settlement stock whenever CE weapons appear.
- Postfix-based patch on `StockGenerator.GenerateThings(Faction)` to minimize conflicts.
- Requires Combat Extended; load this mod after CE.
