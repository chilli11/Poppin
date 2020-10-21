import Service, { inject } from '@ember/service';
import HttpResources from '../utils/http-resources';
import { computed } from '@ember/object';
import { tracked } from '@glimmer/tracking';

export default class CategoriesServiceService extends Service {
	@inject apiService;
	@tracked _categories = [];
	
	@computed('_categories')
	get categories() {
		return this._categories;
	}

	getCategories() {
		return this.apiService.request({
			resource: HttpResources.getCategories
		}).then((response) => {
			this._categories = response || [];
			return response;
		});
	}

	addCategory(category) {
		return this.apiService.request({
			resource: HttpResources.addCategory,
			body: category
		}).then(() => this._categories.push(category));
	}

	updateCategory(category) {
		return this.apiService.request({
			resource: HttpResources.updateCategory,
			body: category
		}).then(() => {
			this._categories = (this._categories || []).map(c => c.slug == category.slug ? category : c);
		});
	}
}
