const userToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjQ2NTU2MTU2LTZiODEtNGI4OS04ZTZmLTBhOWM0ZDhkZDE2ZiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJoaXJlbkB5YWhvby5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJoaXJlbkB5YWhvby5jb20iLCJGdWxsTmFtZSI6IkhpcmVuIFBhdGVsIiwiUm9sZVR5cGUiOiJNYW5hZ2VyIiwianRpIjoiMjNjZGQwNjMtNGYyYi00ZDcwLTljM2UtZmZkYmNlNjAyNWJiIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiTWFuYWdlciIsImV4cCI6MTc1MzA4Mjk5NywiaXNzIjoiTXlBcGkiLCJhdWQiOiJNeUFwaVVzZXJzIn0.dkR5QcE2-2ZVFpOl6fKBMABr4Vc2D919mwSg0R4dyTk"; // 🔐 Replace with actual JWT

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7143/taskhub", {
        accessTokenFactory: () => userToken
    })
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveTask", function (task) {
    const taskItem = document.createElement("li");
    taskItem.textContent = `🆕 ${task.title}: ${task.description} ${task.createdAt}`;
    document.getElementById("taskList").appendChild(taskItem);
    alert(`New Task Assigned: ${task.title}`);
});

async function startConnection() {
    try {
        await connection.start();
        console.log("✅ Connected to SignalR hub");
    } catch (err) {
        console.error("❌ Connection failed: ", err);
        setTimeout(startConnection, 5000);
    }
}

startConnection();