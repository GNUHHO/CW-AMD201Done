<template>
    <div class="container py-5" style="max-width: 700px;">

        <div class="text-center mb-5">
            <h1 class="display-5 fw-bold text-primary">URL Shortener</h1>
            <p class="text-muted">Shorten your link in a snap.</p>
        </div>

        <div class="card shadow-sm border-0 mb-4">
            <div class="card-body p-4">
                <div class="input-group input-group-lg">
                    <input v-model="originalUrl"
                           type="url"
                           class="form-control"
                           placeholder="Dán link dài vào đây..."
                           @keyup.enter="shortenUrl" />
                    <button class="btn btn-success px-4" @click="shortenUrl" :disabled="isLoading">
                        <span v-if="isLoading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                        {{ isLoading ? 'Processing...' : 'Shorten It' }}
                    </button>
                </div>
            </div>
        </div>

        <div v-if="shortUrl" class="alert alert-success shadow-sm d-flex align-items-center justify-content-between">
            <div>
                <h5 class="alert-heading mb-1">Success!</h5>
                <a :href="shortUrl" target="_blank" class="text-decoration-none fw-bold text-success fs-5">
                    {{ shortUrl }}
                </a>
            </div>
            <div class="ms-3 p-2 bg-white rounded shadow-sm">
                <qrcode-vue :value="originalUrl" :size="100" level="H" />
            </div>
        </div>

        <div v-if="errorMessage" class="alert alert-danger shadow-sm">
            {{ errorMessage }}
        </div>

        <div v-if="history.length > 0" class="mt-5">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h4 class="m-0 text-secondary">Abbreviated history</h4>
                <button class="btn btn-sm btn-outline-danger" @click="clearHistory">Delete history</button>
            </div>

            <div class="table-responsive shadow-sm rounded">
                <table class="table table-hover table-bordered mb-0 align-middle">
                    <thead class="table-light">
                        <tr>
                            <th scope="col" style="width: 40%">Shortened link</th>
                            <th scope="col" style="width: 60%">Original link</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in history" :key="index">
                            <td>
                                <a :href="item.shortUrl" target="_blank" class="fw-bold text-primary text-decoration-none">
                                    {{ item.shortUrl }}
                                </a>
                            </td>
                            <td class="text-truncate" style="max-width: 300px;" :title="item.originalUrl">
                                <span class="text-muted">{{ item.originalUrl }}</span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

    </div>
</template>

<script setup>
    import { ref, onMounted } from 'vue';

    import QrcodeVue from 'qrcode.vue';

    const originalUrl = ref('');
    const shortUrl = ref('');
    const errorMessage = ref('');
    const isLoading = ref(false);

    // Biến lưu trữ danh sách lịch sử
    const history = ref([]);

    // Khi trang web vừa mở lên, lấy dữ liệu từ LocalStorage ra hiển thị
    onMounted(() => {
        const savedHistory = localStorage.getItem('myUrlHistory');
        if (savedHistory) {
            history.value = JSON.parse(savedHistory);
        }
    });

    const shortenUrl = async () => {
        if (!originalUrl.value) return;

        isLoading.value = true;
        errorMessage.value = '';
        shortUrl.value = '';

        try {
            const response = await fetch('http://localhost:5078/api/shorten', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ url: originalUrl.value })
            });

            if (!response.ok) throw new Error('Unable to shorten the link. Please check again!');

            const data = await response.json();
            shortUrl.value = data.shortUrl;

            // --- THÊM VÀO LỊCH SỬ ---
            // Đẩy link mới lên đầu danh sách
            history.value.unshift({
                originalUrl: originalUrl.value,
                shortUrl: data.shortUrl
            });

            // Lưu mảng mới vào LocalStorage của trình duyệt
            localStorage.setItem('myUrlHistory', JSON.stringify(history.value));

        } catch (error) {
            errorMessage.value = error.message;
        } finally {
            isLoading.value = false;
        }
    };

    // Hàm xóa lịch sử
    const clearHistory = () => {
        history.value = [];
        localStorage.removeItem('myUrlHistory');
    };
</script>
