namespace Website

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets
open Actions

module Site =
    let PolicyViewPage = 
        Layout.WithTemplate "Policy view" PolicyView <| fun ctx ->
            [
                Div [Text "Policy data"]
            ]

    let PolicyListViewPage =
        Layout.WithTemplate "Policy list view" PolicyListView <| fun ctx ->
            [
                Div [Text "Policy list"]
            ]

    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" PolicyView PolicyViewPage
            Sitelet.Content "List" PolicyListView PolicyListViewPage
        ]

type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [PolicyView; PolicyListView]

[<assembly: WebsiteAttribute(typeof<Website>)>]
do ()
