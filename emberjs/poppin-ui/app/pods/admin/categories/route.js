import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class AdminCategoriesRoute extends Route {
	@service categoriesService;

	model() {
		return this.categoriesService.getCategories();
	}

	@action
	reload() {
		this.refresh();
	}
}
