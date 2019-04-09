var settingdb = db.getSiblingDB('api');
var collection = settingdb.getCollection('setting');
if(collection.count() > 0){
	print('Database has been initialized');
}
else{
    collection.insert({"wrong_password_try_allowed": 5, })
}
function getSettings(){
    var settingdb = db.getSiblingDB('api');
    var collection = settingdb.getCollection('setting');
    return collection.find({})._query
}
function log_wrong_password(ip){
    var collection = db.getSiblingDB('api').getCollection('blacklist');
    var record = collection.findOne({ip:ip, status:{$ne:"remove"}});
    if(record){
        print(record);
        if(record.status == "active")
        {
            //it's already active in the black list
        }
        else if(record.status == "inactive"){
            var settings = getSettings();
            print(settings)
            if(record.try >= settings.wrong_password_try_allowed ){
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
    
    

log_wrong_password("1.1.1.1");
