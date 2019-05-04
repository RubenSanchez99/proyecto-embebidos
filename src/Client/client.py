import requests
#r = requests.post("http://localhost:5108/api/v1/payment/a275fcb7-3873-47ae-bb70-fe3aa46172a7", data={'amount': 50})
r = requests.get("http://localhost:5108/api/v1/payment/a275fcb7-3873-47ae-bb70-fe3aa46172a7")
print(r.status_code, r.reason)
print(r.text[:300] + '...')