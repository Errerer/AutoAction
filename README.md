
# XIVAutoAttack

[![Github Latest Releases](https://img.shields.io/github/downloads/ArchiDog1998/XIVAutoAttack/latest/total.svg?label=最新版本下载量&style=for-the-badge)]()
[![Github All Releases](https://img.shields.io/github/downloads/ArchiDog1998/XIVAutoAttack/total.svg?label=总下载量&style=for-the-badge)]()
[![Github Lines](https://img.shields.io/tokei/lines/github/ArchiDog1998/XIVAutoAttack?label=总行数&style=for-the-badge)]()
[![Github License](https://img.shields.io/github/license/ArchiDog1998/XIVAutoAttack.svg?label=开源协议&style=for-the-badge)]()

如果你喜欢这个插件，可以在这个目录中下载它: 

`https://raw.githubusercontent.com/ArchiDog1998/XIVAutoAttack/master/pluginmaster.json`

QQ交流群：`913297282`，注意，入群问题中的下载量并非上方标签的下载量，请看Dalamud中显示的数值

## 插件概况

本插件提供全职业的自动攻击，可以自动找最优目标，并提供循环教育模式。

![案例](gifs/ExampleDNC.gif)

## 设计原则

> 不降低任何玩家群体的游戏体验。

为达到上述目标最需要保证会因为自动循环而降低游戏体验的群体：`手打玩家`。

手打玩家中会有一部分重视自己在游戏中的各方面表现，如果自动循环能够非常轻易的超过手打玩家的游戏表现，那么就会让一部分纯手打玩家失去手打的乐趣，这不是本插件所期望达到的。 

### 设计宗旨

为此，需要保证所做的插件`不能超过`手打的表现，只能保证游戏表现的`兜底`。

所以对于本插件的循环的要求，仅仅有以下指标：

- 基本满足[警察网](https://xivanalysis.com/)对于循环的要求
- 能打爆除了当前版本以外的`所有等级同步木桩`
- 任何等级下，没有必要能力技`空转`
- 会灵活`切换`AOE和单体攻击适应各种战斗环境
- 能够一定程度上的`自动奶`和`自动上盾`，但不保证不溢出
- 可以随时`调整战术`以及`屏蔽`自动循环

为保证游戏表现不能过高，必须做到以下限制：

- 不能为`任何副本`单独做轴

- 不能精确到每个技能的单独控制

  需要一定的模糊度以区分出自动的整体表现不如手动

### 适用人群
- 不想自己打循环，但是想要`体验副本机制`的玩家。
  - 日常刷日随、幻化的玩家
  - 每周清个CD摸摸鱼的玩家
- 想要如何`学习循环`的玩家
  - 刚刚接手一个职业不会玩，想了解怎么打
  - 练习循环看攻略麻烦，想要哪儿亮了点哪里的玩家



## 插件设计

插件对所有技能进行了细分

- 所有判断主要以自身或敌人状态（buff）、技能冷却时间（cooldown）、职业量谱（gauge）来判断接下来要用什么技能。尽可能少用之前技能的记录、自定义字段等。

