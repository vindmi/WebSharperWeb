module Users
open WebMatrix.WebData

    let login userName password rememberUser =
            match rememberUser with
                | Some b -> WebSecurity.Login(userName, password, b)
                | None -> WebSecurity.Login(userName, password)