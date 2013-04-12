namespace Website
open WebMatrix.WebData
open Users

    type Global() =
        inherit System.Web.HttpApplication()

        member this.Start() =
            WebSecurity.InitializeDatabaseConnection(
                "DatabaseContext", "User", "id", "login", true);

        member this.BeginRequest() = ()
//            register { 
//                login = "U1"
//                password = "a"
//                firstName = "dima"
//                lastName = "v"
//            }
