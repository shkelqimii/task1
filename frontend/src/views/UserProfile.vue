<template>
  <div :class="{ 'dark': isDarkMode }">
    <!-- Navbar -->
    <div class="bg-white dark:bg-gray-900 shadow">
      <div class="max-w-7xl mx-auto py-4 px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center">
          <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">User Dashboard</h1>
          <div class="flex items-center">
            <button @click="goToIndex" class="text-gray-600 dark:text-gray-300 mr-4">
                Index
              </button>
    
            <button @click="toggleDarkMode" class="text-gray-600 dark:text-gray-300 mr-4">
              {{ isDarkMode ? 'Light Mode' : 'Dark Mode' }}
            </button>
            <button @click="logout" class="bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded-lg">
              Logout
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Profile Content -->
    <div class="py-12 bg-gray-100 dark:bg-gray-800 min-h-screen"> <!-- Background change for dark mode -->
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="bg-white dark:bg-gray-700 overflow-hidden shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">User Profile</h1>
            <div v-if="userProfile" class="mt-4">
              <p class="text-lg text-gray-700 dark:text-gray-300">Username: {{ userProfile.username }}</p>
              <!-- Add more user details here -->
            </div>
            <div v-else class="text-lg text-gray-700 dark:text-gray-300">
              Loading...
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const userProfile = ref(null);
const isDarkMode = ref(localStorage.getItem('darkMode') === 'true');

const toggleDarkMode = () => {
  isDarkMode.value = !isDarkMode.value;
  localStorage.setItem('darkMode', isDarkMode.value);
};



const fetchUserProfile = async () => {
  const token = localStorage.getItem('authToken');
  if (!token) {
    router.push('/login');
    return;
  }

  try {
    const response = await fetch('http://localhost:5002/user/profile', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error('Failed to fetch profile');
    }

    userProfile.value = await response.json();
  } catch (error) {
    console.error('Error fetching user profile:', error);
    router.push('/login');
  }
};
const goToIndex = () => {
  router.push('/');
};

onMounted(fetchUserProfile);
</script>
