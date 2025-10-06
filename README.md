**TransConnect - Road Transport Management System**

**Overview**

A Windows Forms desktop application for managing a road transport company with route optimization algorithms and business analytics capabilities.

**Features**

* Complete client and employee management with hierarchical visualization
* Order tracking and management with automated route optimization
* Vehicle fleet management (Cars, Vans, Tankers, Dump Trucks, Refrigerated Trucks)
* Route optimization using three pathfinding algorithms (Dijkstra, Bellman-Ford, Floyd-Warshall)
* Interactive visualizations (city network map, organizational charts)
* Comprehensive analytics dashboard (driver performance, revenue tracking, order statistics)
* Automated distance and cost calculations
* CSV-based data persistence with automatic saving

**Components**

1. **Algorithms/** - Pathfinding implementations and graph traversal algorithms
2. **Data/CSV/** - Client, employee, order, vehicle, and city distance datasets
3. **Models/** - Business entities (Client, Salarie, Commande, Vehicule, Graphe)
4. **Services/** - Business logic layer for data manipulation
5. **UI/** - Windows Forms interfaces for all management modules

**Technologies**

* .NET 8.0 Windows Forms
* C# with MVC-inspired architecture
* Graph algorithms for route optimization


**Algorithm Performance Comparison**

| Algorithm | Complexity | Use Case |
|-----------|-----------|----------|
| Dijkstra | O((V+E) log V) | Single-source shortest path (recommended) |
| Bellman-Ford | O(V×E) | Handles negative weights, cycle detection |
| Floyd-Warshall | O(V³) | All-pairs shortest paths |

**Main Modules**

* **Client Management** - Customer database with order history and spending analysis
* **Employee Management** - Staff hierarchy with interactive organizational chart
* **Order Management** - Delivery creation, assignment, and status tracking
* **Statistics** - Revenue graphs, driver metrics, client analytics
* **Visualization** - City network maps and algorithm comparison tools

**Notes**

* All data is persisted in CSV format in the `Data/CSV/` directory
* First run will load sample data for French cities and transportation network
* Order creation automatically validates driver and vehicle availability by date
* Organizational chart supports zoom, pan, and department filtering