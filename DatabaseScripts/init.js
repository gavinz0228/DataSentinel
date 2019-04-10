use api
var settingdb = db.getSiblingDB('api');
var collection = settingdb.getCollection('setting');
if(collection.count() > 0){
	print('Database has been initialized');
}
else{
    collection.insert({"wrong_password_try_allowed": 5, })
}
var getSettings = function (){
    var settingdb = db.getSiblingDB('api');
    var collection = settingdb.getCollection('setting');
    return collection.findOne({})
}
db.system.js.save(
   {
     _id: "getSettings",
     value : getSettings
   }
)
var isIPBlocked = function (ip){
    var collection = db.getSiblingDB('api').getCollection('blacklist');
    var record = collection.findOne({ip:ip, status: "active"});
    if(record)
        return true;
    else
        return false;
}
db.system.js.save(
   {
     _id: "isIPBlocked",
     value : isIPBlocked
   }
)

var log_wrong_password = function (ip){
    var collection = db.getSiblingDB('api').getCollection('blacklist');
    var record = collection.findOne({ip:ip, status:{$ne:"remove"}});
    if(record){
        if(record.status == "active")
        {
            //it's already active in the black list
        }
        else if(record.status == "inactive"){
            var settings = getSettings();
            if(record.try < settings.wrong_password_allowed ){
                //increase try Number
                record.try = record.try + 1;
                collection.update({ip:ip,status: "inactive"}, record)
            }
            else{
                //make the black list item active
                record.status = "active"
                collection.update({ip:ip,status: "inactive"}, record)
            }
                
    }

}
    else{
        collection.insert({ip:ip, try: 1, status: "inactive" })
    }
}
db.system.js.save(
   {
     _id: "log_wrong_password",
     value : log_wrong_password
   }
)