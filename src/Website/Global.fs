namespace Website
open WebMatrix.WebData

    type Global() =
        inherit System.Web.HttpApplication()

        member this.Start() =
            WebSecurity.InitializeDatabaseConnection(
                "DatabaseContext", "Client", "id", "login", true);

        member this.BeginRequest() = ()
