module Users
open WebMatrix.WebData
open DataModel
open System.Collections.Generic
open System.Linq

    type userData = { 
        login : string 
        password: string
        firstName: string 
        lastName: string 
    }

    let login userName password persistCookie =
            match persistCookie with
                | Some b -> WebSecurity.Login(userName, password, b)
                | None -> WebSecurity.Login(userName, password)

    let logout =
        WebSecurity.Logout()
        let cookie = new System.Web.HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, "")
        cookie.Expires <- System.DateTime.Now.AddYears(-10)
        System.Web.HttpContext.Current.Response.SetCookie(cookie)

    let createUser data =
        let usr = new User(Login = data.login, FirstName = data.firstName, LastName = data.lastName)
        use db = new Website.DatabaseContext()
        db.Users.Add(usr) |> ignore
        db.SaveChanges() 
            |> fun rows -> if rows = 1 then Some(usr) else None

    let createUserAccount userName pwd =
        WebSecurity.CreateAccount(userName, pwd)
        
    let register (data : userData) =
        let usr = createUser data
        if usr.IsSome
            then createUserAccount usr.Value.Login data.password
            else ""