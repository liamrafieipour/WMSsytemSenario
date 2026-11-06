# Stock Allocation

## 1. Overview

This project implements a C# library for allocating warehouse stock to customer orders.

The stock allocation process considers:

- Customer order priorities.
- Order creation dates and FIFO processing.
- Available stock across multiple Stock Keeping Units (SKUs).
- Locked warehouse locations.
- Complete-delivery and partial-delivery requirements.
- Stock shared across multiple order lines and customer orders.
- Order and order-line cancellation.

The solution consists of a class library and a separate xUnit test project.

## 2. Project Structure

### WMSsystemLibrary (StockAllocation)

The class library contains the business logic.

- `Domain` – Represents business objects, rules and logic.
  - `Allocation.cs` – Represents stock allocated from a SKU.
  - `CustomerOrder.cs` – Represents a customer order.
  - `OrderLine.cs` – Represents an individual product request.
  - `OrderPriority.cs` – Defines Low, Normal, and High priorities.
  - `OrderStatus.cs` – Defines the current order status.
  - `StockKeepingUnit.cs` – Represents stock for a product.
  - `WarehouseLocation.cs` – Represents a warehouse location and its lock status.
- `Services` – Coordinates operations involving multiple domain objects.
  - `StockAllocationService.cs` – Contains the allocation and cancellation logic.

### StockAllocation.Tests

Contains unit tests written with xUnit.

The tests are organised into separate folders:

- `Part1_Stock_Allocation_for_Customer_Orders` – Part 1: Stock allocation for customer orders.
- `Part2_OrderCancellation` – Part 2: Order cancellation.

## 3. Part 1 – Stock Allocation

### 3.1 Order Priority

Orders are processed in the following priority order:

1. High
2. Normal
3. Low

Within the same priority, older orders are processed first based on their creation date.


### 3.2 Stock Selection

- Stock is matched using the product number.
- Product numbers are matched exactly and are case-sensitive.
- Stock from multiple SKUs can be combined to fulfil one order line.
- A SKU can supply multiple order lines and multiple customer orders.
- Allocated quantities cannot be allocated again.
- Stock from locked warehouse locations cannot be allocated.
- SKUs with no available quantity are ignored.
- When several matching SKUs are available, they are selected in ascending ordinal order of SKU ID.

### 3.3 Complete Delivery

If `CompleteDeliveryRequired` is true, all remaining order line quantities must be available before any additional stock is allocated to that order.

If any order line cannot be fully fulfilled, no new allocation is made for that order during that allocation run.

### 3.4 Partial Delivery

If `CompleteDeliveryRequired` is false, the service allocates as much available stock as possible.

An order can therefore be partially allocated.

A partially allocated order can be processed again when additional stock becomes available.

### 3.5 Order Status

The order status is updated after allocation:

- `Released` – No stock has been allocated.
- `PartiallyAllocated` – Some stock has been allocated, but the order is not complete.
- `ReadyForDelivery` – All active order lines have been fully allocated.
- `Cancelled` – The order has been cancelled or all its order lines have been cancelled.

## 4. Part 2 – Order Cancellation

The system supports cancellation of entire orders and individual order lines.

### 4.1 Order Cancellation

- An entire order can be cancelled.
- All allocated stock belonging to the order is released.
- All order lines are marked as cancelled.
- Cancelled order lines cannot receive further allocations.
- The order status changes to `Cancelled`.

### 4.2 Order Line Cancellation

- Individual order lines can be cancelled.
- All stock allocated to the cancelled line is released.
- Other active order lines remain unchanged.
- Released stock becomes available for other orders.

### 4.3 Released Stock Reuse

Stock released through cancellation can immediately be allocated to other orders.

## 5. Assumptions and Design Decisions

The following assumptions were made because the assignment does not specify every implementation detail.

1. **Unique SKU IDs:** SKU IDs must be unique within the supplied stock collection.
2. **Order line IDs:** Order line IDs must be unique within an individual customer order.
3. **Product matching:** Product numbers are matched exactly and are case-sensitive.
4. **SKU selection:** Matching SKUs are selected in ascending ordinal order of SKU ID. 
5. **Priority processing:** High priority is processed before Normal, and Normal before Low.
6. **FIFO processing:** Orders with the same priority are processed by creation date, oldest first.
7. **Equal timestamps:** Orders with the same priority and creation date retain their original input order.
8. **Locked locations:** All stock in a locked location is unavailable for allocation.
9. **Partial allocation:** Partial-delivery orders receive as much available stock as possible.
10. **Complete delivery:** Complete-delivery orders receive no new allocation unless all their remaining requirements can be fulfilled.
11. **Repeated allocation:** Existing allocations are retained when the allocation service is run again. Partially allocated orders can receive additional stock in later runs.
12. **Cancellation:** Cancelling an order or order line releases all stock allocated to the cancelled order or line.
13. **Execution:** Allocation is performed sequentially. Concurrent allocation is outside the scope of this implementation.
14. **Persistence:** The library does not store data permanently. Orders and stock are supplied by the caller.
15. **External integrations:** ERP integrations, APIs, user interfaces, databases and other external systems are outside the scope of this implementation.

## 6. Unit Test Coverage

The solution uses xUnit to test the implemented business rules and important edge cases.

### 6.1 Part 1 – Stock Allocation for Customer Orders

Tests cover:

- Basic stock allocation.
- Locked warehouse locations.
- Complete and partial delivery.
- Multiple SKUs.
- Shared stock across orders.
- Order priority and FIFO.
- Product matching.
- Multiple order lines.
- Same-timestamp FIFO behaviour.
- Invalid stock quantities.
- Duplicate SKU IDs.
- Repeated allocation and partial allocation scenarios.

The Part 1 test project contains 19 test cases across 18 test classes.

### 6.2 Part 2 – Order Cancellation

Tests cover:

- Cancelling an entire order.
- Cancelling an individual order line.
- Reusing stock released through cancellation.

## 7. Building and Running Tests

### Requirements

- Visual Studio with .NET development support.
- A compatible .NET SDK for the project target framework.

### 7.1 Build

1. Open `StockAllocation.sln` in Visual Studio.
2. Select **Build → Rebuild Solution**.
3. Check the Error List and ensure the solution builds successfully.

### 7.2 Run Tests

In Visual Studio:

1. Open **Test → Test Explorer**.
2. Select **Run All Tests**.
3. Verify that all tests pass.

## 8. Scope

The implementation covers Parts 1 and 2 of the stock allocation assignment.
The solution is intentionally implemented as a C# class library.
The focus is on implementing and testing the stock allocation and order cancellation business rules.

## 9. Further Improvements

1. **Additional validation:** Improve validation of input collections, including null values and duplicate identifiers, and provide clear error messages.
2. **More edge-case tests:** Add tests for empty stock collections, empty order collections, invalid priorities and repeated allocation attempts.
3. **Improved reporting:** Provide a clearer allocation result showing which quantities were allocated, which remain outstanding, and why an order could not be fully allocated.
4. **Concurrency protection:** Consider how stock allocation should work if multiple allocation processes run simultaneously.
5. **Performance:** Evaluate the allocation process with large numbers of orders and SKUs.