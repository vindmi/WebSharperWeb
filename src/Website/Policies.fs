namespace Website

module Policies =
    open DataAccess

    type PolicyData = {
        constructionType : int
        constructionYear : int
        area : int
        objectType : int
        isFireAlarm : bool
        isBurglaryAlarm : bool
    }

    let private generateNumber () =
        let adapter = new DataAdapter();

        adapter.GetPolicyCount().ToString().PadLeft(8, '0') 

    let Issue (draft:PolicyData) (client:Client) (price:int) =
        let o = new InsuredObject();
        o.area <- int16 draft.area
        o.construction_year <- int16 draft.constructionYear
        o.object_type <- byte draft.objectType
        o.construction_type <- byte draft.constructionType
        o.is_burglary_alarm <- draft.isBurglaryAlarm
        o.is_fire_alarm <- draft.isFireAlarm

        let p = new Policy();
        p.holder_id <- client.id
        p.number <- generateNumber()
        p.InsuredObject <- o
        p.created_at <- System.DateTime.Now
        p.premium <- new System.Nullable<decimal>(decimal price)
        p.status <- byte 1
        p.valid_from <- System.DateTime.Now.AddDays(1.0).Date
        p.valid_tille <- System.DateTime.Now.AddYears(1).Date

        let adapter = new DataAdapter();
        adapter.SavePolicy(p)
 