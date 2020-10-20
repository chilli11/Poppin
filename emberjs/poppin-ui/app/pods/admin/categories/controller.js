import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';

export default class AdminCategoriesController extends Controller {
	@service categoriesService;
	@tracked isUpdate;

	@tracked name;
	@tracked slug;
	@tracked parent;
	@tracked hereId;
	@tracked relatedCSV;
	@tracked childrenCSV

	populateForm(cat) {
		set(this, 'id', cat.id);
		set(this, 'name', cat.name);
		set(this, 'slug', cat.slug);
		set(this, 'parent', cat.parent);
		set(this, 'hereId', cat.hereId);
		set(this, 'relatedCSV', (cat.related || []).join(','));
		set(this, 'childrenCSV', (cat.children || []).join(','));
	}

	@action
	addOrUpdateCategory() {
		const method = this.isUpdate ? 'updateCategory' : 'addCategory';
		const category = {
			id: this.id,
			name: this.name,
			slug: this.slug,
			parent: this.parent,
			hereId: this.hereId,
			related: (this.relatedCSV || '').split(',').map(r => r.trim()),
			children: (this.childrenCSV || '').split(',').map(r => r.trim())
		}
		return this.categoriesService[method](category).then(() => {
			this.clearForm();
			this.send('reload');
		}).catch(e => console.error(e));
	}

	@action
	clickAction(category) {
		this.isUpdate = true;
		this.populateForm(category);
	}

	@action
	clearForm() {
		this.isUpdate = false;
		this.populateForm({});
	}
}
