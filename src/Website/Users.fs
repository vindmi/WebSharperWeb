namespace Website

module Users =
    open WebMatrix.WebData
    open System.Collections.Generic
    open System.Linq
    open DataAccess

    type LoginData = {
        Name : string
        Password : string
    }
    type ClientData = { 
        Name : string
        Password : string
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
                login = data.Name, 
                first_name = data.firstName, 
                last_name = data.lastName,
                code = data.idCode,
                birth_date = data.birthDate)
        let dataAdapter = new DataAdapter();
        dataAdapter.SaveClient(usr)
            |> function
                | true ->  Some(usr) 
                | _ -> None

    let private createUserAccount userName pwd =
        WebSecurity.CreateAccount(userName, pwd)
        
    let Register (data : ClientData) =
        let usr = createUser data
        if usr.IsSome
            then createUserAccount usr.Value.login data.Password |> fun _ -> true
            else false

    let Authenticate userName =
        if WebSecurity.UserExists userName then
            System.Web.Security.FormsAuthentication.SetAuthCookie(userName, false)
        else ()

    let CurrentUser() =
        if WebSecurity.IsAuthenticated then
            let a = new DataAdapter();
            Some (a.FindClient(WebSecurity.CurrentUserId))
        else None