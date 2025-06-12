from flask import Flask, render_template, request, redirect, url_for, jsonify, session, flash
import requests
from flask_cors import CORS

app = Flask(__name__)
app.secret_key = 'your_secret_key'  # Set a secret key for session management
#CORS(app, resources={r"/api/*": {"origins": "http://127.0.0.1:5000"}})  # Enable CORS for all routes
CORS(app, resources={r"/api/*": {"origins": ["http://127.0.0.1:5000", "http://vetsoft.pythonanywhere.com/"]}})  # Enable CORS for all routes

#CORS(app, resources={r"/api/*": {"origins": "https://vetsoft.pythonanywhere.com/"}})

baseURL = 'https://localhost:7001/api/'
#baseURL = 'https://termprojectbackend.azurewebsites.net/api/'

@app.route('/api/proxy', methods=['GET', 'POST'])
def proxy():
    # Get the URL to proxy from the request
    url = request.args.get('url')
    if not url:
        return jsonify({'error': 'URL parameter is required'}), 400
    
    # Forward the request to the external API
    response = requests.get(url)
    
    # Return the response from the external API
    return jsonify(response.json()), response.status_code

@app.route('/')
def home():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('home.html', username=username, role=role)

    return redirect(url_for('login'))

"""
@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        email = request.form['email']
        password = request.form['password']
        for user in users:
            if user['email'] == email and user['password'] == password:
                # Store user_id in session
                session['user_id'] = user['id']
                # Redirect to home page
                return redirect(url_for('home'))
        return jsonify({'error': 'Invalid credentials'}), 401  # Unauthorized status code
    return render_template('login.html')
"""

@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        email = request.form['email']
        password = request.form['password']
        payload = {'email': email, 'password': password}
        try:
            response = requests.post(baseURL + 'Login/Staff', json=payload, verify=False)
        except requests.exceptions.RequestException as e:
            print("Request Exception:", e)
            return jsonify({'error': 'Service unavailable'}), 503
        
        if response.status_code == 200:
            data = response.json()
            user_data = data['user']
            session['user_id'] = user_data['id']
            session['user_name'] = user_data['name']
            session['user_role'] = user_data['role']
            
            session['token'] = data['token']
            session['refreshToken'] = data['refreshToken']
            return redirect(url_for('home'))
        else:
            return jsonify({'error': 'Invalid username or password'}), 401
    return render_template('login.html')

@app.route('/get_users',methods=['GET', 'POST'])
def get_users():
    if request.method == 'GET':
        response = requests.get(baseURL+'UserProfile/GetAllUsers',verify=False,)
        if response.status_code == 200:
            users = response.json()
            print(str(users))
            return jsonify(users), 200
        
@app.route('/get_pet_names/<int:customer_id>')
def get_pet_names(customer_id):
    api_url = f"https://termprojectbackend.azurewebsites.net/api/GetPet/{customer_id}"
    response = requests.get(api_url)
    
    if response.status_code == 200:
        pet_data = response.json()
        pet_names = [pet['name'] for pet in pet_data]
        return jsonify({"customer_id": customer_id, "pet_names": pet_names})
    else:
        return jsonify({"error": "Failed to retrieve pet names"}), response.status_code
    
@app.route('/register', methods=['POST'])
def register():
    email = request.form.get('email')
    password = request.form.get('password')
    confirm_password = request.form.get('confirmPassword')

    if password != confirm_password:
        return jsonify({'success': False, 'message': 'Password does not match confirmation'})
    
    return jsonify({'success': True, 'message': 'Registration successful'})


@app.route('/logout', methods=['GET'])
def logout():
    # Clear the user session
    session.pop('user_id', None)
    return redirect(url_for('home'))

# Sample customers data
customers = [
    {"id": 1, "name": "John Doe"},
    {"id": 2, "name": "Jane Smith"},
    {"id": 3, "name": "Alice Johnson"}
]

# Sample staff data
staff_members = [
    {"id": 1, "name": "John Doe", "email": "john@example.com", "role": "Admin"},
    {"id": 2, "name": "Jane Smith", "email": "jane@example.com", "role": "Staff"}
]

@app.route('/staff')
def staff():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('staff.html', staff_members=staff_members, username=username, role=role)

    return redirect(url_for('login'))


@app.route('/addstaff', methods=['GET', 'POST'])
def addstaff():
    if request.method == 'POST':
        # Get data from the form submission
        name = request.form['name']
        email = request.form['email']
        role = request.form['role']
        password = request.form['password']

        staff_data = {'email': email, 'name': name, 'password': password, 'role': role}
        print(staff_data)
        new_staff_member = {"id": 3, "name": name, "email": email, "role": role}
        
        api_url = 'https://termprojectbackend.azurewebsites.net/VetStaff/CreateStaff'
        response = requests.post(api_url, json=staff_data, verify=False)
        if response.status_code == 200:
            staff_members.append(new_staff_member)
            return render_template('home.html')
        else:
            return render_template('addstaff.html')
        
    return render_template('addstaff.html')

@app.route('/addappointment')
def addappointment():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('addappointment.html', username=username, role=role)

    return redirect(url_for('login'))

@app.route('/vaccination')
def vaccination():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('vaccination.html', username=username, role=role)

    return redirect(url_for('login'))

@app.route('/outofstock')
def outofstock():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('outofstock.html', username=username, role=role)

    return redirect(url_for('login'))

@app.route('/review')
def review():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('review.html', username=username, role=role)

    return redirect(url_for('login'))

@app.route('/pet')
def pet():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('pet.html', username=username, role=role)

    return redirect(url_for('login'))

@app.route('/message', methods=['GET', 'POST'])
def message():
    """
    if request.method == 'POST':
        message = request.form['message']
        username = request.form['customer']
    
        user_id_response = requests.get('https://localhost:7001/api/UserProfile/GetUserIdByUserName', verify=False, params={'userName': username})
        if user_id_response.status_code == 200:
            # Print the content of the response
            print(user_id_response.text)
        else:
            # Print an error message if the request was not successful
            print(f"Error: {user_id_response.status_code}")
        
        print("Message: "+ message)  
        print("Username: "+ username)
        if user_id_response.status_code == 200:
            api_url = 'https://localhost:7001/api/Notification/SendNotification'
            payload = {'userId': user_id_response, 'message': message}
            #response = requests.get('https://localhost:7001/api/Login/LoginForStaff', verify=False)
            #response = requests.post(api_url, verify=False, json=payload)
            """      
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('message.html', customers=customers, username=username, role=role)

    return redirect(url_for('login')) 

# Sample items data
items = [
    {"id": 1, "name": "Medicine A","quantity": 1},
    {"id": 2, "name": "Medicine B","quantity": 1},
    {"id": 3, "name": "Vitamin C","quantity": 1}
]

# Sample appointments data
appointments = [
    {"date": "15-03-2024", "time": "09:00", "customer": "John Doe", "reason": "Routine checkup"},
    {"date": "16-03-2024", "time": "10:30", "customer": "Jane Smith", "reason": "Vaccination"},
    {"date": "17-03-2024", "time": "13:45", "customer": "Alice Johnson", "reason": "Emergency"}
]

@app.route('/appointments')
def view_appointments():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('appointments.html', appointments=appointments, username=username, role=role)

    return redirect(url_for('login'))


@app.route('/stock', methods=['GET', 'POST'])
def stock():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('stock.html', items=items, username=username, role=role)

    return redirect(url_for('login'))

@app.route('/additem')
def additem():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('additem.html', username=username, role=role)

    return redirect(url_for('login'))

@app.route('/checkmessage')
def checkmessage():
    user_id = session.get('user_id')
    if 'user_id' in session:
        user_id = session['user_id']
        """
        user = next((user for user in users if user['id'] == user_id), None
        if user:
        """
        username = user_id = session.get('user_name')
        role = session.get("user_role")
        return render_template('checkmessage.html', username=username, role=role)

    return redirect(url_for('login'))

if __name__ == '__main__':
    app.run(debug=True)