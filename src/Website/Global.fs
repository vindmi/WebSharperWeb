namespace Website
open WebMatrix.WebData

    type Global() =
        inherit System.Web.HttpApplication()

        member this.Start() =
            WebSecurity.InitializeDatabaseConnection(
                "DatabaseContext", "User", "id", "username", true);