# Hello, ASP.NET Core!

参考：
https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api

## 大致流程

建立项目

```PowerShell
dotnet new webapi -o TodoApi
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

添加 Model 类： TodoItem

添加 数据库上下文 类： TodoContext

注册数据库上下文

添加 Controller 脚手架

```PowerShell
# 添加 NuGet 依赖包
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.Design
# 全局安装脚手架引擎（代码生成器）
dotnet tool install --global dotnet-aspnet-codegenerator
# 生成 Controller 脚手架
dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers
```

到这一步就能跑起来玩玩了

后续是一些微调，以及增加 DTO (Data Transfer Object)
