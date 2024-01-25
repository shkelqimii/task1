<template>
  <div class="flex items-center justify-center min-h-screen bg-gradient-to-br from-purple-600 to-blue-500">
    <div class="p-8 max-w-md w-full space-y-8 bg-white rounded-xl shadow-2xl">
      <div class="text-center">
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900">
          Create your account
        </h2>
        <p class="mt-2 text-sm text-gray-600">
          Or <a href="#" class="font-medium text-blue-600 hover:text-blue-500">
            sign in to your existing account
          </a>
        </p>
      </div>
      <form @submit.prevent="register" class="mt-8 space-y-6" action="#" method="POST">
        <input type="hidden" name="remember" value="true">
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
          <button type="submit" class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:bg-indigo-500">
            <span class="absolute left-0 inset-y-0 flex items-center pl-3">
              <!-- Optional: include an icon within the button -->
            </span>
            Register
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

const register = async () => {
  try {
    const response = await fetch('http://localhost:5002/user/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: username.value, 
        password: password.value
      })
    });
    if (response.ok) {
      router.push('/login'); // Redirect to login on success
    } else {
      alert('Registration failed'); // Show error message on failure
    }
  } catch (error) {
    console.error(error);
    alert('An error occurred'); // Show error message on exception
  }
};
</script>
