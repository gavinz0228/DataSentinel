import requests
import json
import os
class DataAccessor(object):
    def __init__(self, base_url):

        self.base_url = self.add_slash(base_url)

        self.token = None
        self.post_headers = {'Content-Type': 'application/json'}
        self.get_headers = {'Content-Type': 'application/json'}

    def post(self, table_name, data):
        if self.token and 'Authorization' not in self.post_headers:
            self.post_headers['Authorization'] = 'Bearer ' + self.token 
        print(self.base_url + 'api/data/post/' + table_name)
        response = requests.post(self.base_url + 'api/data/add/' + table_name, json = data, headers = self.post_headers)
        self.check_status(response)

    def get(self, table_name, filter_str):
        if self.token and 'Authorization' not in self.get_headers:
            self.get_headers['Authorization'] = 'Bearer ' + self.token 
        response = requests.get(self.base_url + 'api/data/get/' + table_name + "/" + json.dumps(filter_str), headers = self.get_headers)
        self.check_status(response)
        return response.json()

    def check_status(self, response):
        if response.status_code != 200:
            raise Exception("host returned error code " + str(response.status_code) + " \r\n" + response.text)

    def auth(self, user_name, password):
        response = requests.post(self.base_url + "api/auth/login", json = {"UserName":user_name, "Password":password}, headers = self.post_headers)
        self.check_status(response)
        resp_json = response.json()
        self.token = resp_json['token']
        if not self.token:
            raise Exception('Wrong user name or password.')
    def add_slash(self, url):
        if not url.endswith('/'):
            return url + '/'
        else:
            return url
if __name__ == "__main__":
    da = DataAccessor("127.0.0.1")
    da.auth(os.environ['BACKEND_USER_NAME'], os.environ['BACKEND_PASSWORD'])
    record = da.get('user', {"a": "c"})
    print(record)