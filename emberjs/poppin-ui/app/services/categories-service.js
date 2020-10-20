import Service, { inject } from '@ember/service';
import HttpResources from '../utils/http-resources';

export default class CategoriesServiceService extends Service {
	@inject apiService;

	getCategories() {
		return this.apiService.request({
			resource: HttpResources.getCategories
		});
	}

	addCategory(category) {
		return this.apiService.request({
			resource: HttpResources.addCategory,
			body: category
		});
	}

	updateCategory(category) {
		return this.apiService.request({
			resource: HttpResources.addCategory,
			body: category
		});
	}
}
