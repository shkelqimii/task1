<template>
  <div :class="isDarkMode ? 'dark' : ''">
    <div class="min-h-screen bg-gray-100 dark:bg-gray-800">
      <div class="bg-white dark:bg-gray-900 shadow">
        <div class="max-w-7xl mx-auto py-4 px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between items-center">
            <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">User Dashboard</h1>
            <div class="flex items-center">
              <button @click="goToProfile" class="flex items-center text-gray-600 dark:text-gray-300 mr-4">
                <img v-if="currentUserProfile.imagePath" :src="getImageUrl(currentUserProfile.imagePath)" alt="Profile Image" class="w-10 h-10 object-cover rounded-full mr-2" />
                Profile
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

      <div class="py-12">
        <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="bg-white dark:bg-gray-700 overflow-hidden shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h2 class="text-lg leading-6 font-medium text-gray-900 dark:text-white mb-4">All Users</h2>
              <div class="border-t border-gray-200 dark:border-gray-600">
                <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-600">
                  <thead class="bg-gray-50 dark:bg-gray-800">
                    <tr>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">ID</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Image</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Username</th>
                      <th class="px-6 py-3 text-right pr-4 text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Username</th>

                    </tr>
                  </thead>
                  <tbody class="bg-white dark:bg-gray-700 divide-y divide-gray-200 dark:divide-gray-600">
                    <tr v-for="user in users" :key="user.id" class="hover:bg-gray-50 dark:hover:bg-gray-600">
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-300">{{ user.id }}</td>
                      <td class="px-6 py-2 whitespace-nowrap text-sm text-gray-500 dark:text-gray-300">
                        <img v-if="user.imagePath" :src="getImageUrl(user.imagePath)" alt="User Image" class="w-10 h-10 object-cover rounded-full" />
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-300">{{ user.username }}</td>
                      <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <button @click="openEditModal(user)" class="mr-4 px-5 py-2 text-white bg-gradient-to-r from-green-500 to-green-700 hover:from-yellow-700 hover:to-yellow-400 shadow-md hover:shadow-lg rounded-full transform transition-all duration-200 hover:scale-110">Edit</button>
                        <button @click="deleteUser(user.id)" class="mr-4 px-5 py-2 text-white bg-gradient-to-r from-red-800 to-pink-600 hover:from-pink-700 hover:to-red-800 shadow-md hover:shadow-lg rounded-full transform transition-all duration-200 hover:scale-110">Delete</button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>


      <div v-if="showEditModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full">
        <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
          <div class="mt-3 text-center">
            <h3 class="text-lg leading-6 font-medium text-gray-900">Edit User</h3>
            <div class="mt-2 px-7 py-3">
              <input v-model="selectedUser.username" placeholder="Username" class="mb-3 px-4 py-2 border rounded-lg text-gray-700 w-full focus:outline-none"/>
              <input type="password" v-model="selectedUser.password" placeholder="Password" class="mb-3 px-4 py-2 border rounded-lg text-gray-700 w-full focus:outline-none"/>
            </div>
            <div class="items-center px-4 py-3">
              <button @click="editUser" class="px-4 py-2 bg-blue-500 text-white text-base font-medium rounded-md shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-300">Save</button>
              <button @click="showEditModal = false" class="px-4 py-2 ml-2 bg-gray-300 text-gray-900 text-base font-medium rounded-md shadow-sm hover:bg-gray-400 focus:outline-none focus:ring-2 focus:ring-gray-300">Cancel</button>
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

const users = ref([]);
const router = useRouter();
const isDarkMode = ref(false);
const showEditModal = ref(false);
const selectedUser = ref({});
const currentUserProfile = ref({}); 



const toggleDarkMode = () => {
  isDarkMode.value = !isDarkMode.value;
  localStorage.setItem('darkMode', isDarkMode.value);
};

const logout = () => {
  localStorage.removeItem('authToken');
  router.push('/login');
};

const fetchUsers = async () => {
  const token = localStorage.getItem('authToken');
  if (!token) {
    logout();
    return;
  }

  try {
    const response = await fetch('http://localhost:5002/user/all', {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      }
    });
    if (response.ok) {
      users.value = await response.json();
    } else {
      alert('Failed to fetch users');
      logout();
    }
  } catch (error) {
    console.error(error);
    alert('An error occurred while fetching users');
    logout();
  }
};

const deleteUser = async (userId) => {
  if (!confirm('Are you sure you want to delete this user?')) {
    return;
  }

  const token = localStorage.getItem('authToken');
  if (!token) {
    logout();
    return;
  }

  try {
    const response = await fetch(`http://localhost:5002/user/delete/${userId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    if (response.ok) {
      alert('User deleted successfully');
      fetchUsers();
    } else {
      alert('Failed to delete user');
    }
  } catch (error) {
    console.error(error);
    alert('An error occurred while deleting the user');
  }
};


const openEditModal = (user) => {
  selectedUser.value = { ...user, password: '' };
  showEditModal.value = true;
};

const editUser = async () => {
  const token = localStorage.getItem('authToken');
  if (!token) {
    logout();
    return;
  }

  try {
    const response = await fetch(`http://localhost:5002/user/edit/${selectedUser.value.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        username: selectedUser.value.username,
        password: selectedUser.value.password
      })
    });
    if (response.ok) {
      alert('User updated successfully');
      fetchUsers();
      showEditModal.value = false;
    } else {
      alert('Failed to update user');
    }
  } catch (error) {
    console.error(error);
    alert('An error occurred while updating the user');
  }
};

const goToProfile = () => {
  router.push('/profile');
};

const getImageUrl = (path) => {
  return `http://localhost:5002/UploadedImages/${path}`;
};

const fetchCurrentUserProfile = async () => {
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

    if (response.ok) {
      currentUserProfile.value = await response.json();
    } else {
      console.error('Failed to fetch current user profile');
    }
  } catch (error) {
    console.error('Error fetching current user profile:', error);
  }
};


onMounted(() => {
  fetchCurrentUserProfile();
  fetchUsers();
  isDarkMode.value = localStorage.getItem('darkMode') === 'true';
});
</script>
