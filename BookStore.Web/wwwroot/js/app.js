import { BookRenderer } from './book.js';
import { ApiService } from './api.js';

class BookGenerator {
    constructor() {
        this.currentPage = 1;
        this.observer = null;

        this.init();
    }

    init() {
        this.apiService = new ApiService();
        this.bookRenderer = new BookRenderer(this.apiService, () => this.getParameters());

        this.setupUI();
        this.setupEventListeners();
        this.setupIntersectionObserver();
        this.loadInitialData();
    }

    setupUI() {
        const languageSelect = document.getElementById('languageSelect');
        const seedInput = document.getElementById('seedInput');
        const likesRange = document.getElementById('likesRange');
        const reviewsInput = document.getElementById('reviewsInput');
        const likesValue = document.getElementById('likesValue');

        languageSelect.innerHTML = '';
        appConfig.regions.forEach(region => {
            const option = document.createElement('option');
            option.value = region.Code;
            option.textContent = region.DisplayName;
            languageSelect.appendChild(option);
        });

        languageSelect.value = appConfig.defaultRegion;
        seedInput.value = appConfig.defaultSeed;
        likesRange.value = appConfig.defaultLikes;
        reviewsInput.value = appConfig.defaultReviews;
        likesValue.textContent = appConfig.defaultLikes;
    }

    setupEventListeners() {
        document.getElementById('generateSeed').addEventListener('click', () => {
            document.getElementById('seedInput').value = Math.floor(Math.random() * 99999999) + 1;
            this.onParametersChange();
        });

        document.getElementById('likesRange').addEventListener('input', (e) => {
            document.getElementById('likesValue').textContent = parseFloat(e.target.value).toFixed(1);
            this.onParametersChange();
        });

        ['languageSelect', 'seedInput', 'reviewsInput'].forEach(id => {
            document.getElementById(id).addEventListener('change', () => {
                this.onParametersChange();
            });
        });

        document.getElementById('exportBtn').addEventListener('click', () => {
            this.exportToCsv();
        });
    }

    setupIntersectionObserver() {
        this.observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !this.apiService.getIsLoading()) {
                    this.loadMoreBooks();
                }
            });
        }, { threshold: 0.1 });

        this.observer.observe(document.getElementById('scrollTrigger'));
    }

    onParametersChange() {
        this.currentPage = 1;
        this.bookRenderer.clearBooks();
        this.loadInitialData().then(() => {
            setTimeout(() => this.checkScrollTrigger(), 100);
        });
    }

    checkScrollTrigger() {
        const trigger = document.getElementById('scrollTrigger');
        const rect = trigger.getBoundingClientRect();
        if (rect.top < window.innerHeight && !this.apiService.getIsLoading()) {
            this.loadMoreBooks();
        }
    }

    async loadInitialData() {
        await this.loadBooks(1);

        checkScrollTrigger();
    }

    async loadMoreBooks() {
        if (this.apiService.getIsLoading()) return;

        this.currentPage++;
        await this.loadBooks(this.currentPage);
    }

    async loadBooks(pageNumber) {
        const parameters = this.getParameters();
        parameters.PageNumber = pageNumber;

        console.log('Sending parameters:', parameters);

        const result = await this.apiService.loadBooks(parameters);
        if (result && result.items) {
            this.bookRenderer.renderBooks(result.items);
        }
    }

    async exportToCsv() {
        const parameters = this.getParameters();
        parameters.PageCount = this.currentPage;

        await this.apiService.exportToCsv(parameters);
    }

    getParameters() {
        return {
            RegionCode: document.getElementById('languageSelect').value,
            UserSeed: parseInt(document.getElementById('seedInput').value) || 1,
            AverageLikes: parseFloat(document.getElementById('likesRange').value) || 0,
            AverageReviews: parseFloat(document.getElementById('reviewsInput').value) || 0
        };
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new BookGenerator();
});