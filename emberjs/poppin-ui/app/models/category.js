import Model, { attr } from '@ember-data/model';

export default class CategoryModel extends Model {
	@attr('string') slug;
	@attr('string') name;
	@attr('string') parent;
	@attr related;
	@attr children;
}