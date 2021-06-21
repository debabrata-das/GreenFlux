@GreenFlux
Feature: GreenFlux Integration Test Scenarios
	Create Group, ChargeStation and Connector via the GreenFlux API endpoint

Scenario: 1 Create new Group
	Given the following Group details are available
		| Name		| Identifier                           | Capacity |
		| Group 1	| d963e6c5-c7d0-4f51-b129-1a3d677803d6 | 100      |
	When the Group data is sent to the Group API endpoint
	Then the Group should be saved successfully

Scenario: 2 Create new ChargeStation linked to the above Group
	Given the following ChargeStation details are available
		| Name				| Identifier                           | GroupIdentifier						| 
		| ChargeStation 1	| 8effd196-4812-4d0f-bc33-0443f537df59 | d963e6c5-c7d0-4f51-b129-1a3d677803d6	|
	When the ChargeStation data is posted to the ChargeStation API endpoint
	Then the ChargeStation should be saved successfully

Scenario: 3 Create new Connector linked to the above ChargeStation
	Given the following Connector details are available
		| Identifier	| ChargeStationIdentifier              | MaxCurrentInAmps	| 
		| 1				| 8effd196-4812-4d0f-bc33-0443f537df59 | 10					|
	When the Connector data is posted to the ChargeStation API endpoint
	Then the Connector should be saved successfully