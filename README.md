# LexLibrary.Rbac

本專案基於
[Role-based access control](https://zh.wikipedia.org/wiki/%E4%BB%A5%E8%A7%92%E8%89%B2%E7%82%BA%E5%9F%BA%E7%A4%8E%E7%9A%84%E5%AD%98%E5%8F%96%E6%8E%A7%E5%88%B6)
實作下列模組

1. 登入 & 登出
2. 註冊 & 驗證 Email
3. 忘記密碼 & 重設密碼
4. 角色模組
5. 功能模組
6. 驗證授權狀態 (可限制登入裝置數量)
7. 基本資料維護

# 專案演示

網址：[https://exfast-lexlibrary-rbac-sample.azurewebsites.net/](https://exfast-lexlibrary-rbac-sample.azurewebsites.net/)  
帳號：admin  
密碼：zxcv1234  
信箱：admin@exfast.me  
部落格：[https://blog.exfast.me/2019/01/netcore-lexlibrary-rbac-based-on-role-based-access-control-implementation-verification-module/](https://blog.exfast.me/2019/01/netcore-lexlibrary-rbac-based-on-role-based-access-control-implementation-verification-module/)  

## 加入專案

### 驗證模組
找到 `Startup.cs` 中的 `ConfigureServices` 方法新增下列程式
````
services.AddLexLibraryRbac(options =>
{
    options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
});

// 請自行實作 ICryptHelper, IEmailSender 介面
services.AddScoped(typeof(ICryptHelper), typeof(CryptHelper));
services.AddScoped(typeof(IEmailSender), typeof(EmailSender));
````

### 預設頁面(選用)
找到 `Startup.cs` 中的 `Configure` 方法新增下列程式
````
app.UseLexLibraryRbacRoute();
````

## 驗證方法

在需要驗證的 Controller 或 Action 上方加入驗證標籤即可
````
[LexLibraryRbacAuthorize(FunctionIds = "1", RoleIds = "2,1")]
````
