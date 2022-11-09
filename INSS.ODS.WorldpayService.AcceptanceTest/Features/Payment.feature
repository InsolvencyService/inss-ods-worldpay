Feature: Get URN

As a Business
I want to Upgrade the World Pay API to .NET6 & move to Azure Functions
So that all ODS Services are migrated to PaaS


Scenario: Create Order
	When a post request is made to initiate payment using 'abck' 'some desc' '10.60' 'test@test.com' 'abcd' 'efgh' 'true' '1 test street' 'AA1 1AA' 'Manchester' '075262671717'
	Then the response status code is OK
	And the content of the url is validated
	
Scenario: Refund Order
	When a post request is made to refund payment using 'abck' '10.60' 
	Then the response status code is OK
	And the refund response content is validated

Scenario: Cancel order
	When a post request is made to cancel payment
	Then the response status code is OK
	And the cancel response content is validated

Scenario: Order Proxy
	When a post request is made to proxy payment
	Then the response status code is 

#Commenting test for update order out as Ben confirmed it's not being used anywhere in the code
#Scenario: Update Order
#	When a post request is made to update order
#	Then the response status code is OK

Scenario: Make Payment
	When a post request is made to make payment
	Then the response status code is OK

Scenario: Get service and dependencies health status
	When a get request for payment health check is sent with /health
	Then the response status code is OK

Scenario: Get service health status
	When a get request for payment health check is sent with /health/ping
	Then the response status code is OK
