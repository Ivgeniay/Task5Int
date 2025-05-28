
import { BookRenderer } from './book.js';
import { ApiService } from './api.js';

class BookGenerator {
    constructor() {
        this.currentPage = 0;
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
        });

        document.getElementById('likesRange').addEventListener('change', () => {
            this.onParametersChange();
        });

        ['languageSelect', 'seedInput', 'reviewsInput'].forEach(id => {
            document.getElementById(id).addEventListener('change', () => {
                this.onParametersChange();
            });
        });

        document.getElementById('tableViewBtn').addEventListener('click', () => {
            this.switchViewMode('table');
        });

        document.getElementById('galleryViewBtn').addEventListener('click', () => {
            this.switchViewMode('gallery');
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
        this.currentPage = 0;
        this.bookRenderer.clearBooks();
        this.loadInitialData();
    }

    async loadInitialData() {
        while (this.shouldLoadMore() && !this.apiService.getIsLoading()) {
            this.currentPage++;
            await this.loadBooks(this.currentPage);
        }
    }

    shouldLoadMore() {
        const trigger = document.getElementById('scrollTrigger');
        const rect = trigger.getBoundingClientRect();
        return rect.top < window.innerHeight;
    }

    async loadMoreBooks() {
        if (this.apiService.getIsLoading()) return;

        this.currentPage++;
        await this.loadBooks(this.currentPage);
    }

    async loadBooks(pageNumber) {
        const parameters = this.getParameters();
        parameters.PageNumber = pageNumber;

        const result = await this.apiService.loadBooks(parameters);
        if (result && result.items) {
            this.bookRenderer.renderBooks(result.items);
        }
    }

    switchViewMode(mode) {
        this.bookRenderer.setViewMode(mode);

        const tableBtn = document.getElementById('tableViewBtn');
        const galleryBtn = document.getElementById('galleryViewBtn');

        if (mode === 'table') {
            tableBtn.className = 'btn btn-primary btn-sm active';
            galleryBtn.className = 'btn btn-outline-primary btn-sm';
        } else {
            tableBtn.className = 'btn btn-outline-primary btn-sm';
            galleryBtn.className = 'btn btn-primary btn-sm active';
        }

        this.bookRenderer.clearBooks();
        this.currentPage = 0;
        this.loadInitialData();
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