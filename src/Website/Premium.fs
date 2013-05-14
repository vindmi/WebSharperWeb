namespace Website

module Premium =
    open Policies

    let Calculate (data:PolicyData) =
        if data.area > 50 then 200
        else 100
