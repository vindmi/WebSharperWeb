namespace Website.DataModel

open System.Data.Entity

    type DatabaseContext() =
        inherit DbContext()

        [<DefaultValue>]
        val mutable clients : DbSet<Client>
        member this.Client with get() = this.clients and set(v) = this.clients <- v

        [<DefaultValue>]
        val mutable policies : DbSet<Policy>
        member this.Policies with get() = this.policies and set(v) = this.policies <- v

        [<DefaultValue>]
        val mutable insuredObjects : DbSet<InsuredObject>
        member this.InsuredObjects with get() = this.insuredObjects and set(v) = this.insuredObjects <- v