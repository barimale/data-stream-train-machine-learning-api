#!/usr/bin/env python
# coding: utf-8

import numpy as np
import requests

# Example NumPy array
data = np.array([1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20])
x = range(100)
# API endpoint
api_url = "https://localhost:7174/NeuralNetwork/TrainNetwork"

# Iterate over the NumPy array and make POST requests
for entry in x:
    payload = {"xs": "1,2,3,4,5" , "ys": "1,1,1,1,1,1,1,1,1,1"}  # Convert entry to a format suitable for the API
    try:
        response = requests.post(api_url, headers={"Content-Type": "application/json"},verify=False, json=payload)
        if response.status_code == 200:
            print(f"Success: {response.json()}")
        else:
            print(f"Failed with status code {response.status_code}: {response.text}")
    except requests.exceptions.RequestException as e:
        print(f"An error occurred: {e}")

