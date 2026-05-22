<?php
session_start();
require_once 'Database.php';

$database = new Database();
$db = $database->getConnection();

$loginError = '';
$loginSuccess = '';
$actionMessage = '';

// 1. HANDLE LOGIN
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST['login'])) {
    $username = $_POST['username'];
    $password = $_POST['password'];

    $query = "SELECT ID, Name, status FROM staff WHERE Name = :username AND password = :password LIMIT 1";
    $stmt = $db->prepare($query);
    $stmt->bindParam(':username', $username);
    $stmt->bindParam(':password', $password);
    $stmt->execute();

    if ($stmt->rowCount() > 0) {
        $row = $stmt->fetch(PDO::FETCH_ASSOC);
        if($row['status'] == 'Active') {
             $_SESSION['user_id'] = $row['ID'];
             $_SESSION['user_name'] = $row['Name'];
             $loginSuccess = "Welcome back, " . $row['Name'] . "!";
        } else {
             $loginError = "Account is inactive. Contact Admin.";
        }
    } else {
        $loginError = "Invalid credentials.";
    }
}

// 2. HANDLE TOGGLE STATUS
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST['toggle_status'])) {
    $toggle_id = $_POST['target_id'];
    $current_status = $_POST['current_status'];
    $new_status = ($current_status == 'Active') ? 'Inactive' : 'Active';
    
    $updateQuery = "UPDATE staff SET status = :status WHERE ID = :id";
    $updateStmt = $db->prepare($updateQuery);
    $updateStmt->execute([':status' => $new_status, ':id' => $toggle_id]);
    $actionMessage = "Status updated successfully!";
}

// 3. HANDLE ADD ACCOUNT
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST['add_account'])) {
    $newName = $_POST['new_name'];
    $newDept = $_POST['new_dept'];
    $newEmail = $_POST['new_email'];
    
    $idQuery = $db->query("SELECT MAX(ID) as max_id FROM staff");
    $nextId = $idQuery->fetch()['max_id'] + 1;

    $insertQuery = "INSERT INTO staff (ID, Name, DeptID, email, password, status) VALUES (:id, :name, :dept, :email, 'password123', 'Active')";
    $insertStmt = $db->prepare($insertQuery);
    $insertStmt->execute([':id' => $nextId, ':name' => $newName, ':dept' => $newDept, ':email' => $newEmail]);
    $actionMessage = "New account added successfully!";
}

// 4. HANDLE UPDATE ACCOUNT
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST['update_account'])) {
    $editId = $_POST['edit_id'];
    $editName = $_POST['edit_name'];
    $editDept = $_POST['edit_dept'];
    $editEmail = $_POST['edit_email'];
    
    $updateQuery = "UPDATE staff SET Name = :name, DeptID = :dept, email = :email WHERE ID = :id";
    $updateStmt = $db->prepare($updateQuery);
    $updateStmt->execute([':name' => $editName, ':dept' => $editDept, ':email' => $editEmail, ':id' => $editId]);
    $actionMessage = "Account profile updated successfully!";
}

// 5. FETCH USERS
$searchQuery = "";
if (isset($_GET['search']) && !empty($_GET['search'])) {
    $searchQuery = $_GET['search'];
    $userQuery = "SELECT staff.ID, staff.Name, staff.DeptID, depts.Name AS DeptName, staff.email, staff.status 
                  FROM staff LEFT JOIN depts ON staff.DeptID = depts.ID WHERE staff.Name LIKE :search";
    $userStmt = $db->prepare($userQuery);
    $userStmt->bindParam(':search', $searchTerm);
    $searchTerm = "%" . $searchQuery . "%";
} else {
    $userQuery = "SELECT staff.ID, staff.Name, staff.DeptID, depts.Name AS DeptName, staff.email, staff.status 
                  FROM staff LEFT JOIN depts ON staff.DeptID = depts.ID ORDER BY staff.ID ASC";
    $userStmt = $db->prepare($userQuery);
}
$userStmt->execute();
$usersList = $userStmt->fetchAll(PDO::FETCH_ASSOC);
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>CrispyTrack System</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <style>
        .form-container { display: none; }
        .form-container.active { display: block; }
    </style>
</head>
<body class="bg-orange-50 font-sans text-slate-900">

    <nav class="bg-white border-b border-orange-200 p-4 flex gap-4 justify-center sticky top-0 z-50 shadow-sm flex-wrap">
        <button onclick="showForm('login')" class="px-4 py-2 bg-orange-100 text-orange-800 rounded hover:bg-red-600 hover:text-white font-bold text-xs uppercase transition">1. Login</button>
        <button onclick="showForm('recovery')" class="px-4 py-2 bg-orange-100 text-orange-800 rounded hover:bg-red-600 hover:text-white font-bold text-xs uppercase transition">2. Recovery</button>
        <button onclick="showForm('dashboard')" class="px-4 py-2 bg-orange-100 text-orange-800 rounded hover:bg-red-600 hover:text-white font-bold text-xs uppercase transition">3. Dashboard</button>
        <button onclick="showForm('report')" class="px-4 py-2 bg-orange-100 text-orange-800 rounded hover:bg-red-600 hover:text-white font-bold text-xs uppercase transition">4. Report</button>
        <button onclick="showForm('users')" class="px-4 py-2 bg-red-100 text-red-800 rounded border-2 border-red-500 hover:bg-red-600 hover:text-white font-black text-xs uppercase shadow-sm transition">5. User Mgt</button>
        <button onclick="showForm('about')" class="px-4 py-2 bg-orange-100 text-orange-800 rounded hover:bg-red-600 hover:text-white font-bold text-xs uppercase transition">6. About</button>
    </nav>

    <div class="p-8">
        
        <div id="login" class="form-container <?php echo (!isset($_GET['search']) && empty($actionMessage)) ? 'active' : ''; ?>">
            <div class="max-w-md mx-auto mt-12 p-10 bg-white rounded-3xl shadow-2xl border-t-8 border-red-600">
                <div class="text-center mb-8">
                    <img src="https://thumbs.dreamstime.com/b/design-suitable-product-cover-logo-labels-banner-print-etc-hot-crispy-fried-chicken-logo-template-144595163.jpg?w=768" alt="Logo" class="w-24 h-24 mx-auto rounded-full mb-4 shadow-md">
                    <h2 class="text-3xl font-black text-slate-800 italic">CRISPY TRACK</h2>
                </div>
                
                <?php if(!empty($loginError)): ?>
                    <div class="bg-red-100 text-red-700 p-3 rounded mb-4 text-center font-bold text-sm"><?php echo $loginError; ?></div>
                <?php endif; ?>
                <?php if(!empty($loginSuccess)): ?>
                    <div class="bg-green-100 text-green-700 p-3 rounded mb-4 text-center font-bold text-sm"><?php echo $loginSuccess; ?></div>
                <?php endif; ?>

                <form method="POST" action="index.php" class="space-y-5">
                    <input type="text" name="username" placeholder="Username (e.g., Allen)" class="w-full p-4 bg-slate-50 border border-slate-200 rounded-xl" required>
                    <input type="password" name="password" placeholder="Access Key (password123)" class="w-full p-4 bg-slate-50 border border-slate-200 rounded-xl" required>
                    <button type="submit" name="login" class="w-full bg-red-600 text-white p-4 rounded-xl font-black text-lg shadow-lg hover:bg-red-700 transition">SIGN IN</button>
                </form>
            </div>
        </div>

        <div id="recovery" class="form-container">
            <div class="max-w-md mx-auto mt-12 p-10 bg-white rounded-3xl shadow-xl">
                <h2 class="text-2xl font-bold mb-2">Reset Portal</h2>
                <p class="text-slate-500 mb-8">Enter your staff email to reset your POS access key.</p>
                <div class="space-y-4">
                    <input type="email" id="recoveryEmail" placeholder="staff.email@crispytrack.com" class="w-full p-4 border border-slate-200 rounded-xl">
                    <button onclick="sendRecovery()" class="w-full bg-slate-900 text-white p-4 rounded-xl font-bold hover:bg-black transition">Send Reset Link</button>
                </div>
            </div>
        </div>

        <div id="dashboard" class="form-container">
            <div class="max-w-5xl mx-auto bg-white rounded-3xl shadow-xl overflow-hidden border border-orange-100">
                <div class="bg-red-600 p-8 text-white flex justify-between items-center">
                    <div>
                        <h2 class="text-2xl font-black italic">BRANCH COMMAND CENTER</h2>
                    </div>
                </div>
                <div class="p-10 grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
                    <div class="p-8 bg-white border-2 border-orange-100 rounded-2xl">
                        <h3 class="text-slate-400 font-bold uppercase text-xs">Cooking Now</h3>
                        <p class="text-5xl font-black text-red-600 mt-2">14</p>
                    </div>
                    <div class="p-8 bg-white border-2 border-orange-100 rounded-2xl">
                        <h3 class="text-slate-400 font-bold uppercase text-xs">Total Revenue</h3>
                        <p class="text-5xl font-black text-red-600 mt-2">₱12.4k</p>
                    </div>
                    <div class="p-8 bg-white border-2 border-orange-100 rounded-2xl">
                        <h3 class="text-slate-400 font-bold uppercase text-xs">Staff Present</h3>
                        <p class="text-5xl font-black text-red-600 mt-2">11</p>
                    </div>
                </div>
            </div>
        </div>

        <div id="report" class="form-container">
            <div class="max-w-2xl mx-auto bg-white p-12 rounded-3xl shadow-xl text-center">
                <h2 class="text-3xl font-black mb-4">DATA EXPORT MODULE</h2>
                <button class="w-full bg-green-600 text-white px-8 py-5 rounded-2xl font-black text-xl mt-8">EXPORT TO MS EXCEL (.XLSX)</button>
            </div>
        </div>

        <div id="users" class="form-container <?php echo (isset($_GET['search']) || !empty($actionMessage)) ? 'active' : ''; ?>">
            <div class="max-w-6xl mx-auto bg-white rounded-3xl shadow-xl overflow-hidden border border-orange-100">
                <div class="bg-slate-900 p-8 text-white flex justify-between items-center flex-wrap gap-4">
                    <h2 class="text-2xl font-black italic">ACCOUNT MANAGEMENT</h2>
                    <button onclick="document.getElementById('addForm').classList.toggle('hidden'); document.getElementById('editForm').classList.add('hidden');" class="bg-green-500 hover:bg-green-600 px-4 py-2 rounded font-bold text-sm shadow-lg">+ Add New Account</button>
                </div>
                
                <div class="p-8">
                    <?php if(!empty($actionMessage)): ?>
                        <div class="bg-blue-100 text-blue-800 p-4 rounded-xl mb-6 font-bold text-center border border-blue-200"><?php echo $actionMessage; ?></div>
                    <?php endif; ?>

                    <div id="addForm" class="hidden bg-slate-50 p-6 rounded-xl border border-slate-200 mb-8">
                        <h3 class="font-black text-lg mb-4 text-slate-700">Create New User</h3>
                        <form method="POST" action="index.php" class="flex gap-4 flex-wrap">
                            <input type="text" name="new_name" placeholder="Staff Name" class="p-3 border rounded-lg flex-1 min-w-[200px]" required>
                            <input type="email" name="new_email" placeholder="Email Address" class="p-3 border rounded-lg flex-1 min-w-[200px]" required>
                            <select name="new_dept" class="p-3 border rounded-lg flex-1 min-w-[200px]" required>
                                <option value="1">Kitchen</option>
                                <option value="2">Counter</option>
                                <option value="3">Delivery</option>
                                <option value="4">Storage</option>
                            </select>
                            <button type="submit" name="add_account" class="bg-green-600 text-white px-6 py-3 rounded-lg font-bold hover:bg-green-700">Save Account</button>
                        </form>
                    </div>

                    <div id="editForm" class="hidden bg-orange-50 p-6 rounded-xl border border-orange-200 mb-8">
                        <h3 class="font-black text-lg mb-4 text-orange-700">Update User Profile</h3>
                        <form method="POST" action="index.php" class="flex gap-4 flex-wrap">
                            <input type="hidden" name="edit_id" id="edit_id">
                            <input type="text" name="edit_name" id="edit_name" class="p-3 border rounded-lg flex-1 min-w-[200px]" required>
                            <input type="email" name="edit_email" id="edit_email" class="p-3 border rounded-lg flex-1 min-w-[200px]" required>
                            <select name="edit_dept" id="edit_dept" class="p-3 border rounded-lg flex-1 min-w-[200px]" required>
                                <option value="1">Kitchen</option>
                                <option value="2">Counter</option>
                                <option value="3">Delivery</option>
                                <option value="4">Storage</option>
                            </select>
                            <button type="submit" name="update_account" class="bg-orange-600 text-white px-6 py-3 rounded-lg font-bold hover:bg-orange-700">Update</button>
                            <button type="button" onclick="document.getElementById('editForm').classList.add('hidden')" class="bg-slate-300 text-slate-700 px-6 py-3 rounded-lg font-bold hover:bg-slate-400">Cancel</button>
                        </form>
                    </div>

                    <form method="GET" action="index.php" class="flex gap-4 mb-6">
                        <input type="text" name="search" value="<?php echo htmlspecialchars($searchQuery); ?>" placeholder="Search by name..." class="flex-1 p-3 border rounded-xl bg-slate-50 outline-none focus:ring-2 focus:ring-orange-500">
                        <button type="submit" class="bg-blue-600 text-white px-6 py-3 rounded-xl font-bold hover:bg-blue-700 transition">Search</button>
                        <a href="index.php" class="bg-slate-200 text-slate-700 px-6 py-3 rounded-xl font-bold hover:bg-slate-300 transition text-center pt-3">Clear</a>
                    </form>

                    <div class="overflow-x-auto">
                        <table class="w-full text-left border-collapse min-w-[800px]">
                            <thead>
                                <tr class="bg-orange-100 text-orange-800 uppercase text-xs tracking-widest">
                                    <th class="p-4 rounded-tl-xl">ID</th>
                                    <th class="p-4">Name</th>
                                    <th class="p-4">Department</th>
                                    <th class="p-4">Email</th>
                                    <th class="p-4">Status</th>
                                    <th class="p-4 rounded-tr-xl text-center">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                <?php foreach($usersList as $user): ?>
                                <tr class="border-b border-slate-100 hover:bg-slate-50 transition">
                                    <td class="p-4 font-bold text-slate-500"><?php echo htmlspecialchars($user['ID']); ?></td>
                                    <td class="p-4 font-bold text-slate-800"><?php echo htmlspecialchars($user['Name']); ?></td>
                                    <td class="p-4"><?php echo htmlspecialchars($user['DeptName']); ?></td>
                                    <td class="p-4 text-slate-500"><?php echo htmlspecialchars($user['email']); ?></td>
                                    <td class="p-4">
                                        <span class="px-3 py-1 text-xs font-bold rounded-full <?php echo ($user['status'] == 'Active') ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'; ?>">
                                            <?php echo htmlspecialchars($user['status']); ?>
                                        </span>
                                    </td>
                                    <td class="p-4 flex gap-2 justify-center">
                                        <button onclick="openEditForm(<?php echo $user['ID']; ?>, '<?php echo addslashes($user['Name']); ?>', '<?php echo addslashes($user['email']); ?>', <?php echo $user['DeptID']; ?>)" class="bg-blue-100 text-blue-700 px-4 py-2 rounded font-bold text-xs hover:bg-blue-200 transition">Edit</button>
                                        <form method="POST" action="index.php" style="display:inline;">
                                            <input type="hidden" name="target_id" value="<?php echo $user['ID']; ?>">
                                            <input type="hidden" name="current_status" value="<?php echo $user['status']; ?>">
                                            <button type="submit" name="toggle_status" class="bg-slate-200 text-slate-700 px-4 py-2 rounded text-xs font-bold hover:bg-slate-300 transition">Toggle Status</button>
                                        </form>
                                    </td>
                                </tr>
                                <?php endforeach; ?>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <div id="about" class="form-container">
            <div class="max-w-2xl mx-auto bg-white rounded-3xl shadow-2xl overflow-hidden border border-orange-100">
                <div class="bg-gradient-to-r from-red-600 to-orange-500 p-10 text-center text-white">
                    <img src="https://thumbs.dreamstime.com/b/design-suitable-product-cover-logo-labels-banner-print-etc-hot-crispy-fried-chicken-logo-template-144595163.jpg?w=768" alt="Logo" class="w-32 h-32 mx-auto rounded-full mb-4 bg-white p-1 shadow-2xl">
                    <h2 class="text-4xl font-black tracking-tight italic">CRISPY TRACK InfoSystem</h2>
                    <p class="opacity-90 mt-2 font-bold uppercase text-xs tracking-widest">Enterprise Fast-Food Management v1.0.3</p>
                </div>
                
                <div class="p-10">
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-10 text-left">
                        <div>
                            <h4 class="text-xs font-bold text-red-600 uppercase tracking-widest mb-4">System Logic</h4>
                            <p class="text-slate-600 text-sm leading-relaxed mb-4">
                                CrispyTrack is a specialized POS Information System built on the <code>fried_chicken</code> schema. It manages order flows across multiple departments including Kitchen, Counter, and Delivery.
                            </p>
                        </div>

                        <div>
                            <h4 class="text-xs font-bold text-red-600 uppercase tracking-widest mb-4">Database Features</h4>
                            <ul class="text-slate-600 text-sm space-y-4">
                                <li class="leading-relaxed">
                                    <span class="font-bold text-slate-800">• Trigger Automation:</span> 
                                    Updates <code>dept_summary</code> automatically on staff changes.
                                </li>
                                <li class="leading-relaxed">
                                    <span class="font-bold text-slate-800">• Financial Logic:</span> 
                                    Deterministic <code>AddTax</code> function for consistent 10% pricing.
                                </li>
                                <li class="leading-relaxed">
                                    <span class="font-bold text-slate-800">• Relational Views:</span> 
                                    Optimized data display via <code>view_salessummary</code>.
                                </li>
                            </ul>
                        </div>
                    </div>

                    <div class="mt-12 pt-8 border-t border-slate-100 text-center">
                        <h4 class="text-xs font-bold text-slate-400 uppercase tracking-widest mb-4">Lead Developer</h4>
                        <div class="flex flex-col items-center mb-8">
                            <p class="text-2xl font-black text-slate-800 uppercase tracking-tighter">Rein Allen M. Capitulo</p>
                            <p class="text-red-600 font-bold text-sm">Bicol University College of Science</p>
                        </div>
                        
                        <div class="grid grid-cols-2 gap-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">
                            <div class="text-right border-r border-slate-200 pr-4">IT 106 - EDP</div>
                            <div class="text-left pl-4 text-red-500">Dr. Jayvee Christopher N. Vibar</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script>
        function showForm(id) {
            document.querySelectorAll('.form-container').forEach(f => f.classList.remove('active'));
            document.getElementById(id).classList.add('active');
        }

        function openEditForm(id, name, email, dept) {
            document.getElementById('editForm').classList.remove('hidden');
            document.getElementById('addForm').classList.add('hidden');
            
            document.getElementById('edit_id').value = id;
            document.getElementById('edit_name').value = name;
            document.getElementById('edit_email').value = email;
            document.getElementById('edit_dept').value = dept;
            
            // Scroll to the edit form
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        function sendRecovery() {
            let email = document.getElementById('recoveryEmail').value;
            if(email === "") {
                alert("Please enter an email address first.");
            } else {
                alert("A password recovery link has been sent to " + email + ". Please check your inbox.");
                document.getElementById('recoveryEmail').value = ""; // clear field
            }
        }
    </script>
</body>
</html>