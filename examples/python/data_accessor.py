import requests
import json
class DataAccessor(object):
    def __init__(self, base_url):
        self.base_url = base_url
        self.token = None
        self.post_headers = {'Content-Type': 'application/json'}
        self.get_headers = {'Content-Type': 'application/json'}
        if not self.base_url.endswith('/'):
            self.base_url += '/'

    def post(self, url, data):
        if self.token and 'Authorization' not in self.post_headers:
            self.post_headers['Authorization'] = 'Bearer ' + self.token 
        response = requests.post(self.base_url + url, json = data, headers = self.post_headers)
        return response.json()

    def get(self, url, filter_str):
        if self.token and 'Authorization' not in self.get_headers:
            self.get_headers['Authorization'] = 'Bearer ' + self.token 
        print(self.base_url + url + "/" + json.dumps(filter_str))
        response = requests.get(self.base_url + url + "/" + json.dumps(filter_str), headers = self.get_headers)
        return response.json()

    def auth(self, user_name, password):
        resp_json = self.post("api/auth/login", {"UserName":user_name, "Password":password})
        self.token = resp_json['token']
        if not self.token:
            raise Exception('Wrong user name or password.')
            
if __name__ == "__main__":
    da = DataAccessor("http://localhost/")
    da.auth("gavin0228", "8524560")
    record = da.get('api/data/get/user', {"key": "value"})
    print(record)