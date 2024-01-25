<template>
  <div class="flex items-center justify-center min-h-screen bg-gradient-to-br from-purple-600 to-blue-500">
    <div class="p-8 max-w-md w-full space-y-8 bg-white rounded-xl shadow-2xl">
      <div class="text-center">
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900">
          User Login
        </h2>
      </div>
      <form @submit.prevent="login" class="mt-8 space-y-6">
        <div class="rounded-md shadow-sm -space-y-px">
          <div>
            <label for="username" class="sr-only">Username</label>
            <input v-model="username" id="username" name="username" type="text" required class="first:rounded-t-md last:rounded-b-md block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm" placeholder="Username">
          </div>
          <div>
            <label for="password" class="sr-only">Password</label>
            <input v-model="password" id="password" name="password" type="password" required class="first:rounded-t-md last:rounded-b-md block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm" placeholder="Password">
          </div>
        </div>

        <div>
          <button type="submit" class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
            Login
          </button>
        </div>
      </form>
    </div>
  </div>
</template>


<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const username = ref('');
const password = ref('');
const router = useRouter();

const login = async () => {
  try {
    const response = await fetch('http://localhost:5002/user/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: username.value, 
        password: password.value
      })
    });
    if (response.ok) {
      const data = await response.json();
      localStorage.setItem('authToken', data.token);
      router.push('/');
    } else {
      alert('Login failed. Please check your username and password.');
    }
  } catch (error) {
    console.error(error);
    alert('An error occurred');
  }
};
</script>

