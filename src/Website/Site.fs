namespace Website

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets

type Action =
    | PolicyView
    | PolicyListView

module Site =

    let ConstructPage title body =
        PageContent <| fun context ->
            {
                Page.Default with
                    Title = Some title
                    Body =
                        [ H3 [Text <| body ]]
            }

    let PolicyViewPage
        = ConstructPage "Policy view" "Policy data"

    let PolicyListViewPage
        = ConstructPage "Policy list view" "Policy list"

    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" PolicyView PolicyViewPage
            Sitelet.Content "/List" PolicyListView PolicyListViewPage
        ]

type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [PolicyView; PolicyListView]

[<assembly: WebsiteAttribute(typeof<Website>)>]
do ()

//    let ( => ) text url =
//        A [HRef url] -< [Text text]

//    let Links (ctx: Context<Action>) =
//        UL [
//            LI ["Home" => ctx.Link PolicyView]
//            LI ["About" => ctx.Link PolicyListView]
//        ]
//    let HomePage =
//        Skin.WithTemplate "HomePage" <| fun ctx ->
//            [
//                Div [Text "HOME"]
//                Links ctx
//            ]
//
//    let AboutPage =
//        Skin.WithTemplate "AboutPage" <| fun ctx ->
//            [
//                Div [Text "ABOUT"]
//                Links ctx
//            ]
