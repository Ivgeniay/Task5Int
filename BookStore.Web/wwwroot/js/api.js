export class ApiService {
    constructor() {
        this.isLoading = false;
    }

    async loadBooks(parameters) {
        if (this.isLoading) return null;

        this.isLoading = true;
        this.showLoading(true);

        try {
            const response = await fetch('/api/books', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(parameters)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Error loading books:', error);
            this.showError('Failed to load books. Please try again.');
            return null;
        } finally {
            this.isLoading = false;
            this.showLoading(false);
        }
    }

    async loadBookDetails(bookIndex, parameters) {
        try {
            const queryString = new URLSearchParams(parameters).toString();

            const response = await fetch(`/api/book/${bookIndex}?${queryString}`);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const bookDetails = await response.json();
            return bookDetails;
        } catch (error) {
            console.error('Error loading book details:', error);
            this.showError('Failed to load book details. Please try again.');
            return null;
        }
    }

    async exportToCsv(parameters) {
        try {
            const response = await fetch('/api/export', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(parameters)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const blob = await response.blob();
            this.downloadFile(blob, 'books.csv');
        } catch (error) {
            console.error('Error exporting data:', error);
            this.showError('Failed to export data. Please try again.');
        }
    }

    downloadFile(blob, filename) {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
    }

    showLoading(show) {
        const indicator = document.getElementById('loadingIndicator');
        if (indicator) {
            indicator.style.display = show ? 'block' : 'none';
        }
    }

    showError(message) {
        console.error(message);
    }

    getIsLoading() {
        return this.isLoading;
    }
}