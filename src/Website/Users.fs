namespace Website

module Users =
    open WebMatrix.WebData
    open System.Collections.Generic
    open System.Linq
    open Website.DataModel

        type LoginData = {
            Name : string
            Password : string
        }
        type UserData = { 
            login : LoginData
            firstName: string 
            lastName: string
            birthDate : System.DateTime
            idCode: string
        }

        let IsAuthenticated() =
            WebSecurity.IsAuthenticated

        let Login userName password persistCookie =
            match persistCookie with
                | Some b -> WebSecurity.Login(userName, password, b)
                | None -> WebSecurity.Login(userName, password)

        let Logout() =
            WebSecurity.Logout()

        let private createUser data =
            let usr = 
                new Client(
                    Login = data.login.Name, 
                    FirstName = data.firstName, 
                    LastName = data.lastName,
                    Code = data.idCode,
                    BirthDate = data.birthDate)
            use db = new DatabaseContext()
            db.Client.Add(usr) |> ignore
            db.SaveChanges() 
                |> fun rows -> if rows = 1 then Some(usr) else None

        let private createUserAccount userName pwd =
            WebSecurity.CreateAccount(userName, pwd)
        
        let Register (data : UserData) =
            let usr = createUser data
            if usr.IsSome
                then createUserAccount usr.Value.Login data.login.Password
                else ""