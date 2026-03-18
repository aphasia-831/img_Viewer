<p align="center">
  
  🖼️ <b>Image Tag Viewer</b><br><br>
  ===
  <!-- GitHub Star & Fork -->
  <a href="https://github.com/aphasia-831/img_Viewer">
    <img src="https://img.shields.io/github/stars/aphasia-831/img_Viewer?style=social" alt="GitHub stars"/>
  </a>
  <a href="https://github.com/aphasia-831/img_Viewer/fork">
    <img src="https://img.shields.io/github/forks/aphasia-831/img_Viewer?style=social" alt="GitHub forks"/>
  </a>
  <!-- License -->
   <a href="LICENSE">
  <img src="https://img.shields.io/badge/license-MIT-green" alt="License"/></a>
  <!-- 技术栈 -->
   <a href="#"><img src="https://img.shields.io/badge/C%23-WinUI-blue" alt="C#"/></a> <a href="#"> <img src="https://img.shields.io/badge/Python-AI-yellow" alt="Python"/></a> <a href="#"> <img src="https://img.shields.io/badge/SQLite-DB-lightgrey" alt="SQLite"/></a>
</p>
一个基于 WinUI 3 + Python + SQLite 的本地图像标签管理工具，支持自动识别图片内容并进行高效筛选与浏览。

A local image tag management tool based on WinUI 3 + Python + SQLite, supporting automatic image tagging and efficient browsing.

✨ 项目简介 | Overview
----

Image Tag Viewer 是一个面向本地图片管理的工具，结合深度学习模型，实现：

Image Tag Viewer is a local image management tool powered by deep learning models:

 - 自动提取图片标签（人物 / 场景 / 风格等）

   Automatically extract image tags (character / scene / style)

 - 本地数据库存储与快速检索

   Local database storage and fast querying

 - 图像浏览与筛选

   Image browsing and filtering

适用于：

Suitable for:

 - 插画收藏管理 Illustration Collection Management

 - 图像素材整理 Organizing Image Assets

🚀 功能特性 | Features
-----

📂 添加并扫描本地图片文件夹
Scan local image folders

🏷️ 自动生成多标签（基于 illust2vec）
Auto tag generation (based on illust2vec)

🗄️ 使用 SQLite 存储标签数据
Store tags using SQLite

🔍 按标签筛选图片
Filter images by tags

🖼️ 图片预览与浏览
Image preview and browsing

⚡ 本地运行，无需网络
Fully offline

🧱 技术栈 | Tech Stack
----

| 技术 / Technology | 说明 / Description       |
|-----------------|------------------------|
| C#              | 主程序开发 / Main program development |
| WinUI 3         | Windows UI 框架 / Windows UI framework |
| Python          | 图像识别 / Image recognition |
| illust2vec      | 插画标签模型 / Tagging model |
| SQLite          | 本地数据库 / Local database |

 🧠 模型来源 | Model Source
 ----

本项目使用以下开源模型：
This project uses the following open-source model:

- **illust2vec**
  - 作者 / Author: Preferred Networks
  - 项目地址 / Project: [https://github.com/rezoo/illustration2vec](https://github.com/rezoo/illustration2vec)
  - License: 模型遵循原作者 License / The model follows the original author's License

⚠️ 注意：
- 本仓库**不包含模型文件**


⚠️ 模型下载 | Model Download
-----
本仓库不包含illust2vec模型  
The model file (~180MB) is NOT included in this repository.  
下载地址 / Download:  
https://github.com/rezoo/illustration2vec/releases  

📌 注意事项 | Notes
----
- 仅用于学习用途 For educational use only
- 模型版权归原作者 Model belongs to original authors
- 请勿用于商业用途或非法用途


 🛠️ 开发进度 | Progress
 ---

| 状态 | 模块 / Module |
|------|--------------------------|
| ✅ 已完成 / Completed | 图片浏览模块 / Image browsing module<br>标签存储与翻译 / Tag storage & Translation<br>自动标签识别 / Auto tag recognition |
| 🔄 进行中 / In Progress | 标签检索 / Tag search |
| ⏳ 待做 / To Do | 模型自动下载 / Auto model download<br>标签自定义 / Custom Tags<br>多风格识别 / Multi-Style Recognition<br>文件夹监控 / Folder Watch<br>图片收藏 / Favorites<br>未定 / Pending |
