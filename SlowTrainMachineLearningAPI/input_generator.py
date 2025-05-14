#!/usr/bin/env python
# coding: utf-8

import numpy as np
import requests
from sklearn.datasets import fetch_california_housing
from sklearn.model_selection import train_test_split

housing = fetch_california_housing()
Xtrain, Xtest, ytrain, ytest = train_test_split(housing.data, housing.target, test_size = .2)
Xtrain, Xval, ytrain, yval = train_test_split(Xtrain, ytrain, test_size=.2)

x = range(10)
# API endpoint
api_url = "https://localhost:7174/NeuralNetwork/TrainNetwork"

# Iterate over the NumPy array and make POST requests
for entry in x:
    xs = np.random.randn(1,5)*100
    vectorized_floatXs = np.vectorize(int)
    xs = vectorized_floatXs(xs)
    resultXs = np.array2string(xs, separator=',', suppress_small=True)[1:-1].replace(' ', '').replace('\n', '')
    resultXs = resultXs.strip("[")
    resultXs = resultXs.strip("]")
    resultXs = resultXs.strip()
    print(resultXs)
    ys = np.random.randn(1,10)*100
    vectorized_floatYs = np.vectorize(int)
    ys = vectorized_floatYs(ys)
    resultYs = np.array2string(ys, separator=',', suppress_small=True)[1:-1].replace(' ', '').replace('\n', '')
    resultYs = resultYs.strip("[")
    resultYs = resultYs.strip("]")
    resultYs = resultYs.strip()
    print(resultYs)
    payload = {"xs": resultXs , "ys": resultYs}  # Convert entry to a format suitable for the API
    try:
        response = requests.post(api_url, headers={"Content-Type": "application/json"},verify=False, json=payload)
        if response.status_code == 200:
            print(f"Success: {response.json()}")
        else:
            print(f"Failed with status code {response.status_code}: {response.text}")
    except requests.exceptions.RequestException as e:
        print(f"An error occurred: {e}")

