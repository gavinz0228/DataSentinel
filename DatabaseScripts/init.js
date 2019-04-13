use api
var settingdb = db.getSiblingDB('api');
var collection = settingdb.getCollection('setting');
if(collection.count() > 0){
	print('Database has been initialized');
}
else{
    collection.insert({"wrong_password_try_allowed": 5, })
}
