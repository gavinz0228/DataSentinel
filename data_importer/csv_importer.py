from data_accessor import DataAccessor
import os

class CSVImporter(object):
    def __init__(self, file_path, delimiter, backend_url, table_name, header_row_num = 1):
        self.file_path = file_path
        self.delimiter = delimiter
        self.header_row_num = header_row_num
        self.data_accessor = DataAccessor(backend_url)
        self.data_accessor.auth(os.environ['BACKEND_USER_NAME'], os.environ['BACKEND_PASSWORD'])
        self.table_name = table_name

    def import_file(self):
        headers = []
        with open(self.file_path, "r") as f:
            line = f.readline()
            row_num = 1
            #read headers
            header_reached = False
            while line:
                if row_num == self.header_row_num:
                    headers = line.split(self.delimiter)
                    if not headers:
                        raise Exception("No headers are found, assuming file is empty.")
                    header_reached = True
                    line = f.readline()
                    break
                row_num += 1
                line = f.readline()
            if not header_reached:
                raise Exception("Reached the end of the file before reaching the header row " + str(self.header_row_num)) + "."
            #read content
            while line:
                values = line.split(self.delimiter)
                if not values:
                    continue
                pairs = zip(headers, values)
                row_data = { header:value for header, value in pairs}
                print(row_data)
                self.data_accessor.post(self.table_name, row_data)
                line = f.readline()

if __name__ == "__main__":
    importer = CSVImporter("../datasets/forestfires.csv", ",", "127.0.0.1", "forestfires")
    importer.import_file()
