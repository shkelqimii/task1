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
    <div class="py-12 bg-gray-100 dark:bg-gray-800 min-h-screen">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="bg-white dark:bg-gray-700 overflow-hidden shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">User Profile</h1>
            <div v-if="userProfile" class="mt-4">
              <p class="text-lg text-gray-700 dark:text-gray-300">Username: {{ userProfile.username }}</p>
                      <!-- Image Display Section -->
              <div class="mt-4">
                <img v-if="userProfile.imagePath" :src="getImageUrl(userProfile.imagePath)" alt="Profile Image" class="w-32 h-32 object-cover rounded-full" />
                <form @submit.prevent="uploadImage">
                  <input type="file" @change="handleFileChange" />
                  <button type="submit" class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded mt-2">
                    Upload Image
                  </button>
                </form>
              </div>
              
              <!-- Profile Update Form -->
              <div class="mt-4">
                <input v-model="newUsername" type="text" placeholder="New Username" class="border p-2 mb-2 w-full">
                <input v-model="newPassword" type="password" placeholder="New Password" class="border p-2 mb-2 w-full">
                <button @click="updateProfile" class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                  Update Profile
                </button>
              </div>
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
const newUsername = ref('');
const newPassword = ref('');
const selectedFile = ref(null);
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

const updateProfile = async () => {
  const token = localStorage.getItem('authToken');
  if (!token) {
    router.push('/login');
    return;
  }

  try {
    const response = await fetch('http://localhost:5002/user/update-profile', {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        newUsername: newUsername.value,
        newPassword: newPassword.value
      })
    });

    if (!response.ok) {
      throw new Error('Failed to update profile');
    }

    await fetchUserProfile();
    alert('Profile updated successfully');
  } catch (error) {
    console.error('Error updating profile:', error);
  }
};

const uploadImage = async () => {
  if (!selectedFile.value) {
    alert("Please select an image file.");
    return;
  }

  const formData = new FormData();
  formData.append('file', selectedFile.value);

  try {
    const response = await fetch('http://localhost:5002/user/upload-image', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      },
      body: formData
    });

    if (!response.ok) {
      throw new Error('Failed to upload image');
    }

    alert('Image uploaded successfully');
    await fetchUserProfile(); // Refetch user profile to update the image
  } catch (error) {
    console.error('Error uploading image:', error);
  }
};

const handleFileChange = (event) => {
  selectedFile.value = event.target.files[0];
};

const getImageUrl = (path) => {
  return `http://localhost:5002/UploadedImages/${path}`;
};

const goToIndex = () => {
  router.push('/');
};

const logout = () => {
  localStorage.removeItem('authToken');
  router.push('/login');
};

onMounted(fetchUserProfile);
</script>
