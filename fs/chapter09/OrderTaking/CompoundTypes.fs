module OrderTaking.CompoundTypes

open SimpleTypes

type PersonalName = {
    FirstName : String50
    LastName : String50
}

type CustomerInfo = {
    Name : PersonalName
    EmailAddress : EmailAddress
}

type Address = {
    AddressLine1 : String50
    AddressLine2 : String50 option
    AddressLine3 : String50 option
    AddressLine4 : String50 option
    City : String50
    ZipCode : ZipCode
}

