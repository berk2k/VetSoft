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
            #session['refreshToken'] = data['refreshToken']
            return redirect(url_for('home'))
        else:
            return jsonify({'error': 'Invalid username or password'}), 401
    return render_template('login.html')